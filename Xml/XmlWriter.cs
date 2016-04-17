using System;
using System.Collections.Generic;
using System.Text;

namespace Squid.Xml
{
    public class XmlWriter
    {
        class Element
        {
            public bool isRoot;
            public string name;
            public Dictionary<string, string> attributes = new Dictionary<string, string>();
            public string value;

            public List<Element> elements = new List<Element>();
            public Element parent;
        }

        StringBuilder builder;
        List<Element> elements = new List<Element>();
        Element current = new Element { isRoot = true };

        bool indention;

        public XmlWriter(StringBuilder output, bool indent)
        {
            indention = indent;
            builder = output;
        }

        public void Flush()
        {
            foreach(Element element in current.elements)
                Write(element, 0);           
        }

        private void Write(Element element, int level)
        {
            if (level > 0 && indention)
                builder.Append(new string(' ', level * 4));

            builder.Append("<" + element.name);
            
            if (element.attributes.Count > 0)
            {
                builder.Append(" ");

                int i = 0;
                foreach (KeyValuePair<string, string> pair in element.attributes)
                {
                    i++;
                    if(i == element.attributes.Count)
                        builder.Append(pair.Key + "='" + pair.Value + "'");
                    else
                        builder.Append(pair.Key + "='" + pair.Value + "' ");
                }
            }

            bool isEmpty = string.IsNullOrEmpty(element.value) && element.elements.Count == 0;

            if (!isEmpty)
            {
                builder.Append(">");

                if (!string.IsNullOrEmpty(element.value))
                {
                    builder.Append(element.value);
                }
                else
                {
                    if(indention)
                        builder.Append("\r\n");
                    
                    foreach (Element child in element.elements)
                        Write(child, level + 1);
                }

                if (string.IsNullOrEmpty(element.value) && level > 0 && indention)
                    builder.Append(new string(' ', level * 4)); 
                
                builder.Append("</" + element.name + ">\r\n");
            }
            else
                builder.Append("/>\r\n");
        }

        public void Close() { }

        public void WriteStartElement(string name)
        {
            Element e = new Element();
            e.name = name;
            e.parent = current;
         
            current.elements.Add(e);
            current = e;
        }

        public void WriteAttributeString(string name, string value)
        {
            current.attributes.Add(name, value);
        }

        public void WriteEndElement()
        {
            current = current.parent;
        }

        public void WriteValue(string value)
        {
            current.value = value;
        }
    }
}
