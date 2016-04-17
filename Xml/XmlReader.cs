using System.Collections;
using System.Collections.Generic;
using System;
using System.Text.RegularExpressions;

namespace Squid.Xml
{
    public enum XmlNodeType
    {
        Element,
        Text,
        EndElement
    }

    public class XmlAttribute
    {
        public string Name;
        public string Value;
    }

    public class XmlReader
    {
        private string xmlString = "";
        private int idx = 0;
        private XmlNodeType _nodeType;

        public XmlNodeType NodeType { get { return _nodeType; } }
        public bool HasAttributes { get; private set; }

        private List<XmlAttribute> Attributes = new List<XmlAttribute>();
        private int AttributeIndex = -1;
        private bool _isEmptyElement;

        public XmlReader(string xml)
        {
            xmlString = xml;
            _nodeType = XmlNodeType.Text;
        }

        public void New(string xml)
        {
            xmlString = xml;
            _nodeType = XmlNodeType.Text;

            idx = 0;
            Name = "";
            Value = "";
            _isEmptyElement = false;
            AttributeIndex = -1;
            Attributes.Clear();
            HasAttributes = false;
        }
    
        public string Name = "";
        public string Value = "";

        // properly looks for the next index of _c, without stopping at line endings, allowing tags to be break lines   
        int IndexOf(char _c, int _i)
        {
            int i = _i;
            while (i < xmlString.Length)
            {
                if (xmlString[i] == _c)
                    return i;

                ++i;
            }

            return -1;
        }

        public bool EOF
        {
            get { return idx < 0; }
        }

        public bool IsEmptyElement
        {
            get { return _isEmptyElement; }
        }

        public string GetAttribute(string name)
        {
            foreach (XmlAttribute att in Attributes)
            {
                if (att.Name.Equals(name))
                    return att.Value;
            }

            return string.Empty;
        }

        public bool MoveToNextAttribute()
        {
            AttributeIndex++;

            if (AttributeIndex < Attributes.Count)
            {
                Name = Attributes[AttributeIndex].Name;
                Value = Attributes[AttributeIndex].Value;
            }

            return AttributeIndex < Attributes.Count;
        }

        public bool Read()
        {
            int newindex = idx;

            if (idx > -1)
               newindex = xmlString.IndexOf("<", idx);

            Name = string.Empty;
            Value = string.Empty;
            HasAttributes = false;
            Attributes.Clear();
            AttributeIndex = -1;
            _isEmptyElement = false;

            if (newindex != idx)
            {
                if (newindex == -1)
                {
                    if (idx > 0) idx++;

                    Value = xmlString.Substring(idx, xmlString.Length - idx);
                    _nodeType = XmlNodeType.Text;
                    idx = newindex;
                    return true;
                }
                else
                {
                    if (idx > 0) idx++;

                    Value = xmlString.Substring(idx, newindex - idx);
                    _nodeType = XmlNodeType.Text;
                    idx = newindex;
                    return true;
                }
            }

            if (idx == -1)
                return false;
            
            ++idx;

            // skip attributes, don't include them in the name!
            int endOfTag = IndexOf('>', idx);
            int endOfName = IndexOf(' ', idx);
            if ((endOfName == -1) || (endOfTag < endOfName))
            {
                endOfName = endOfTag;
            }

            if (endOfTag == -1)
            {
                return false;
            }

            Name = xmlString.Substring(idx, endOfName - idx);

            idx = endOfTag;

            // check if a closing tag
            if (Name.StartsWith("/"))
            {
                _isEmptyElement = false;
                _nodeType = XmlNodeType.EndElement;
                Name = Name.Remove(0, 1); // remove the slash
            }
            else if(Name.EndsWith("/"))
            {
                _isEmptyElement = true;
                _nodeType = XmlNodeType.Element;
                Name = Name.Replace("/", ""); // remove the slash  
            }
            else
            {
                string temp = xmlString.Substring(endOfName, endOfTag - endOfName);

                Regex r = new Regex("([a-z0-9]+)=(\"(.*?)\")");

                foreach (Match m in r.Matches(temp))
                {
                    string name = m.Value.Substring(0, m.Value.IndexOf("="));
                    int i0 = m.Value.IndexOf("\"") + 1;
                    int i1 = m.Value.LastIndexOf("\"");
                    string val = m.Value.Substring(i0, i1 - i0);

                    Attributes.Add(new XmlAttribute { Name = name, Value = val });
                }

                r = new Regex("([a-z0-9]+)=('(.*?)')");

                foreach (Match m in r.Matches(temp))
                {
                    string name = m.Value.Substring(0, m.Value.IndexOf("="));
                    int i0 = m.Value.IndexOf("'") + 1;
                    int i1 = m.Value.LastIndexOf("'");
                    string val = m.Value.Substring(i0, i1 - i0);

                    Attributes.Add(new XmlAttribute { Name = name, Value = val });
                }

                HasAttributes = Attributes.Count > 0;

                _nodeType = XmlNodeType.Element;
            }

            return idx < xmlString.Length;
        }
    }
}