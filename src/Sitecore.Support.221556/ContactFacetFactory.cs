using Sitecore;
using Sitecore.Analytics.Model.Framework;
using Sitecore.Analytics.Tracking;
using Sitecore.Configuration;
using Sitecore.Diagnostics;
using Sitecore.WFFM.Abstractions.Analytics;
using Sitecore.Xml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml;

namespace Sitecore.WFFM.Analytics
{
  public class ContactFacetFactory : IContactFacetFactory
  {
    private readonly Dictionary<Type, Type> _typeMap;

    public Dictionary<string, IFacet> ContactFacets
    {
      get;
    }

    public ContactFacetFactory()
    {
      string path = "model/entities/contact/facets";
      string path2 = "model/elements";
      _typeMap = LoadFacetsTypeMap(path2);
      ContactFacets = GetFacets(path);
    }

    public void SetFacetValue(Contact contact, string key, string facetXpath, string facetValue, bool overwrite = true)
    {
      Assert.ArgumentNotNull(contact, "contact");
      Assert.ArgumentNotNullOrEmpty(facetXpath, "facetXpath");
      Assert.ArgumentNotNull(facetValue, "facetValue");
      string text = facetXpath.Split('/')[0];
      IFacet facet = contact.Facets[text];
      SetFacetMember(facet, GetFacetMembers(facet), key, facetXpath.Remove(0, text.Length + 1), facetValue, overwrite);
      PropertyInfo property = facet.GetType().GetProperty("Preferred");
      if (property != (PropertyInfo)null && string.IsNullOrEmpty(property.GetValue(facet) as string))
      {
        property.SetValue(facet, key);
      }
    }

    public IElement CreateElement(Type type)
    {
      return (IElement)Activator.CreateInstance(_typeMap[type]);
    }

    public IEnumerable<IModelMember> GetFacetMembers(IElement element)
    {
      Assert.ArgumentNotNull(element, "element");
      PropertyInfo property = element.GetType().GetProperty("Members", BindingFlags.Instance | BindingFlags.NonPublic);
      if (!(property != (PropertyInfo)null))
      {
        return null;
      }
      return property.GetValue(element) as IModelMemberCollection;
    }

    private Dictionary<Type, Type> LoadFacetsTypeMap(string path)
    {
      Assert.ArgumentNotNullOrEmpty(path, "path");
      XmlNode configNode = Factory.GetConfigNode(path);
      Dictionary<Type, Type> dictionary = new Dictionary<Type, Type>();
      if (configNode != null)
      {
        {
          foreach (XmlNode childNode in configNode.ChildNodes)
          {
            if (childNode.Name == "element")
            {
              Type type = Type.GetType(XmlUtil.GetAttribute("interface", childNode, true));
              Assert.IsNotNull(type, "attribute");
              Type type2 = Type.GetType(XmlUtil.GetAttribute("implementation", childNode, true), true);
              dictionary.Add(type, type2);
            }
          }
          return dictionary;
        }
      }
      return dictionary;
    }

    private IEnumerable<KeyValuePair<string, Type>> LoadFacetMap(string path)
    {
      Assert.ArgumentNotNullOrEmpty(path, "path");
      XmlNode configNode = Factory.GetConfigNode(path);
      Dictionary<string, Type> dictionary = new Dictionary<string, Type>();
      if (configNode != null)
      {
        {
          foreach (XmlNode childNode in configNode.ChildNodes)
          {
            if (childNode.Name == "facet")
            {
              string attribute = XmlUtil.GetAttribute("name", childNode, true);
              Type type = Type.GetType(XmlUtil.GetAttribute("contract", childNode, true), true);
              dictionary.Add(attribute, type);
            }
          }
          return dictionary;
        }
      }
      return dictionary;
    }

    private void SetFacetMember(IElement facet, IEnumerable<IModelMember> members, string key, string path, string value, bool overwrite = true)
    {
      Assert.ArgumentNotNull(facet, "members");
      Assert.ArgumentNotNull(members, "members");
      Assert.ArgumentNotNullOrEmpty(path, "path");
      Assert.ArgumentNotNullOrEmpty(path, "key");
      Assert.ArgumentNotNull(value, "value");
      Type type = facet.GetType();
      string memberName = path.Split('/')[0];
      IModelMember modelMember = members.FirstOrDefault((IModelMember x) => x.Name == memberName);
      Assert.IsNotNull(modelMember, "memberFiled");
      if (string.Equals(memberName, "Entries", StringComparison.OrdinalIgnoreCase))
      {
        IElementDictionary<IElement> elementDictionary = type.GetProperty("Entries", BindingFlags.Instance | BindingFlags.Public).GetValue(facet) as IElementDictionary<IElement>;
        Assert.IsNotNull(elementDictionary, "Can't get facet entries.");
        IElement element;
        if (elementDictionary.Keys.FirstOrDefault((string x) => string.Equals(x, key, StringComparison.InvariantCultureIgnoreCase)) != null)
        {
          element = elementDictionary[key];
        }
        else
        {
          object obj = elementDictionary.GetType().GetMethod("Create", BindingFlags.Instance | BindingFlags.Public).Invoke(elementDictionary, new object[1]
          {
                    key
          });
          Assert.IsNotNull(obj, "Can't create entry.");
          element = (obj as IElement);
        }
        SetFacetMember(element, GetFacetMembers(element), key, path.Remove(0, memberName.Length + 1), value, overwrite);
      }
      if (modelMember is IModelAttributeMember)
      {
        SetAttribute(facet, modelMember.Name, value, overwrite);
      }
      if (modelMember is IModelElementMember)
      {
        object value2 = type.GetProperty(memberName, BindingFlags.Instance | BindingFlags.Public).GetValue(facet);
        Assert.IsNotNull(value2, $"Can't get facet element {memberName}.");
        IElement element2 = value2 as IElement;
        SetFacetMember(element2, GetFacetMembers(element2), key, path.Remove(0, memberName.Length + 1), value, overwrite);
      }
    }

    private void SetAttribute(IElement element, string name, string value, bool overwrite)
    {
      Assert.ArgumentNotNull(element, "element");
      Assert.ArgumentNotNullOrEmpty(name, "name");
      PropertyInfo property = element.GetType().GetProperty(name, BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.Public);
      Assert.IsNotNull(property, "attribute");
      if (property.GetValue(element) != null && !overwrite)
      {
        return;
      }
      if (property.PropertyType.IsGenericType)
      {
        DateTime dateTime = DateUtil.IsoDateToDateTime(value);
        Assert.IsNotNull(dateTime, "Date");
        property.SetValue(element, System.Convert.ChangeType(dateTime, property.PropertyType.GetGenericArguments().First()));
      }
      else
      {
        property.SetValue(element, System.Convert.ChangeType(value, property.PropertyType));
      }
    }

    private Dictionary<string, IFacet> GetFacets(string path)
    {
      IEnumerable<KeyValuePair<string, Type>> enumerable = LoadFacetMap(path);
      Dictionary<string, IFacet> dictionary = new Dictionary<string, IFacet>();
      foreach (KeyValuePair<string, Type> item in enumerable)
      {
        string key = item.Key;
        if (!dictionary.ContainsKey(key) && _typeMap.ContainsKey(item.Value))
        {
          IFacet facet = (IFacet)Activator.CreateInstance(_typeMap[item.Value]);
          if (facet != null)
          {
            dictionary.Add(key, facet);
          }
        }
      }
      return dictionary;
    }
  }
}