using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Collections;
using System.Reflection;
using System.IO;
using Squid;

namespace Squid.Xml
{
    public class XmlIgnoreAttribute : Attribute { }

    internal class Node
    {
        public Node Parent;
        public bool WriteType;
        public string Name;
        public string Type;
        public string Value;
        public string Key;
        public int RefID;
        public int Reference;

        public List<Node> Nodes = new List<Node>();
    }

    public class XmlSerializer : IDisposable
    {
        private Node Root;

        private Dictionary<int, object> ReadCache = new Dictionary<int, object>();
        private Dictionary<object, Node> WriteCache = new Dictionary<object, Node>();

        private int Increment;

        public string Serialize(object data)
        {
            CreateLogicalTree(data);

            StringBuilder output = new StringBuilder();
            XmlWriter writer = new XmlWriter(output, true);

            Write(Root, writer);

            writer.Flush();
            writer.Close();
            writer = null;

            return output.ToString();
        }

        public T Deserialize<T>(string xml)
        {
            XmlReader reader = new XmlReader(xml);

            Root = ReadXml(reader);

            reader = null;

            ReadCache.Clear();

            return (T)Deserialize(Root, typeof(T));
        }

        private void CreateLogicalTree(object data)
        {
            try
            {
                WriteCache.Clear();
                Root = CreateLogicalNode(data, false, false);
                WriteCache.Clear();
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        private Node CreateLogicalNode(object data, bool writeType, bool useCache)
        {
            Type type = data.GetType();

            Node result = new Node { Name = type.Name, Type = type.FullName, WriteType = writeType };

            if (WriteCache.ContainsKey(data))
            {
                if (useCache)
                    return WriteCache[data];

                if (WriteCache[data].RefID == 0)
                {
                    Increment++;
                    WriteCache[data].RefID = Increment;
                }

                result.WriteType = false;
                result.Reference = WriteCache[data].RefID;
                return result;
            }
            else
            {
                WriteCache.Add(data, result);
            }

            PropertyInfo[] properties = Reflector.GetProperties(type);// type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            if (properties.Length < 1) result.Value = data.ToString();

            foreach (PropertyInfo info in properties)
            {
                if (info.IsDefined(typeof(XmlIgnoreAttribute), true)) continue;
                if (!info.CanRead) continue;
                if (!info.CanWrite) continue;
                if (info.GetSetMethod() == null) continue;

                object value = info.GetValue(data, null);
                if (value == null) continue;

                // check vs. default value
                object[] attributes = info.GetCustomAttributes(typeof(DefaultValueAttribute), true);
                if (attributes.Length > 0)
                {
                    DefaultValueAttribute def = attributes[0] as DefaultValueAttribute;
                    if (def.Value != null)
                    {
                        if (def.Value.Equals(value))
                            continue;
                    }
                }

                Type valueType = value.GetType();

                if (info.PropertyType.IsPrimitive || info.PropertyType == typeof(string) || info.PropertyType.IsEnum)
                {
                    FlagsAttribute flags = Reflector.GetAttribute<FlagsAttribute>(info.PropertyType);

                    if (flags != null)
                    {
                        Node sub = new Node { Name = info.Name, Value = ((int)value).ToString() };
                        result.Nodes.Add(sub);
                    }
                    else
                    {
                        Node sub = new Node { Name = info.Name, Value = value.ToString() };
                        result.Nodes.Add(sub);
                    }
                }
                else if (valueType.IsValueType)
                {
                    TypeConverter converter = System.ComponentModel.TypeDescriptor.GetConverter(valueType);
                    if (converter != null)
                    {
                        try
                        {
                            string str = converter.ConvertToString(value);
                            Node sub = new Node { Name = info.Name, Value = str };
                            result.Nodes.Add(sub);
                        }
                        catch (Exception exc)
                        {
                            throw exc;
                        }
                    }
                    else
                    {
                        Node sub = CreateLogicalNode(value, !info.PropertyType.FullName.Equals(valueType.FullName), false);
                        sub.Name = info.Name;
                        result.Nodes.Add(sub);
                    }
                }
                else if (value is IList)
                {
                    Node sub = new Node { Name = info.Name, Type = valueType.FullName };
                    result.Nodes.Add(sub);

                    string name = null;
                    string fullname = null;

                    if (valueType.IsGenericType || valueType.BaseType.IsGenericType)
                    {
                        Type t = valueType.IsGenericType ? valueType : valueType.BaseType;

                        Type[] gens = t.GetGenericArguments();
                        if (gens.Length > 0)
                        {
                            name = gens[0].Name;
                            fullname = gens[0].FullName;
                        }
                    }
                    else
                        fullname = ((IList)value)[0].GetType().FullName;

                    foreach (object item in ((IList)value))
                    {
                        if (item != null)
                        {
                            Type stype = item.GetType();
                            Node child = CreateLogicalNode(item, !fullname.Equals(item.GetType().FullName), false);

                            if (!string.IsNullOrEmpty(name))
                                child.Name = name;

                            sub.Nodes.Add(child);
                        }
                        else
                        {
                            Node child = new Node { Name = name };
                            sub.Nodes.Add(child);
                        }
                    }
                }
                else if (value is IDictionary)
                {
                    Node sub = new Node { Name = info.Name, Type = valueType.FullName };
                    result.Nodes.Add(sub);

                    Type itemType = null;
                  
                    if (valueType.IsGenericType)
                    {
                        Type[] gens = valueType.GetGenericArguments();
                        if (gens.Length > 0)
                            itemType = gens[1];
                    }
                    else if (valueType.BaseType.IsGenericType)
                    {
                        Type[] gens = valueType.BaseType.GetGenericArguments();
                        if (gens.Length > 0)
                            itemType = gens[1];
                    }

                    IDictionary dict = (IDictionary)value;
                   
                    foreach (object key in dict.Keys)
                    {
                        Node element = CreateLogicalNode(dict[key], true, false);
                        //Node element = CreateLogicalNode(dict[key], !dict[key].GetType().FullName.Equals(itemType.FullName), false);
                        element.Key = key.ToString();
                        sub.Nodes.Add(element);
                    }
                }
                //else if (value is Entity)// && info.GetAttribute<NoRefAttribute>() == null)
                //{
                //    // entity as a property - we want a reference here, not the actual entity
                //    Node sub = new Node { Name = info.Name, WriteType = false };

                //    if (WriteCache.ContainsKey(value))
                //        sub.Reference = WriteCache[value].RefID;
                //    else
                //    {
                //        Node cached = CreateNode(value, false, false);

                //        if (WriteCache[value].RefID == 0)
                //        {
                //            Increment++;
                //            WriteCache[value].RefID = Increment;
                //        }

                //        sub.Reference = cached.RefID;
                //    }

                //    result.Nodes.Add(sub);
                //}
                else
                {
                    Node sub = CreateLogicalNode(value, !info.PropertyType.FullName.Equals(valueType.FullName), false);
                    sub.Name = info.Name;
                    result.Nodes.Add(sub);
                }
            }

            if (data is Control)
            {
                //    Entity entity = data as Entity;

                //    if (!entity.IsPrototype)
                //    {
                //        Node sub = new Node { Name = "Components", Type = typeof(Component).FullName };
                //        result.Nodes.Add(sub);

                //        foreach (Component c in entity.Components)
                //        {
                //            Node child = CreateNode(c, true, false, resources);
                //            child.Name = "Component";
                //            sub.Nodes.Add(child);
                //        }

                ElementCollection elements = ((Control)data).GetElements();

                if (elements.Count > 0)
                {
                    Node sub = new Node { Name = "Elements", Type = typeof(Control).FullName };
                    result.Nodes.Add(sub);

                    foreach (Control e in elements)
                        sub.Nodes.Add(CreateLogicalNode(e, false, true));
                }
                //    }
                //    else
                //    {
                //        Node sub = new Node { Name = "Components", Type = typeof(Component).FullName };
                //        result.Nodes.Add(sub);
                //        Node child = CreateNode(entity.Transform, true, false, resources);
                //        child.Name = "Component";
                //        sub.Nodes.Add(child);
                //    }
            }

            return result;
        }

        private void Write(Node node, XmlWriter writer)
        {
            writer.WriteStartElement(node.Name);

            if (node.WriteType) writer.WriteAttributeString("xtype", node.Type);
            if (!string.IsNullOrEmpty(node.Key)) writer.WriteAttributeString("xkey", node.Key);
            if (node.RefID != 0) writer.WriteAttributeString("xrefid", node.RefID.ToString());
            if (node.Reference != 0) writer.WriteAttributeString("xref", node.Reference.ToString());

            if (node.Value != null)
                writer.WriteValue(node.Value);
            else
            {
                foreach (Node sub in node.Nodes)
                    Write(sub, writer);
            }

            writer.WriteEndElement();
        }

        private Node ReadXml(XmlReader reader)
        {
            try
            {
                Node current = null;
                Root = null;

                while (!reader.EOF)
                {
                    if (!reader.Read()) return Root;

                    switch (reader.NodeType)
                    {
                        case XmlNodeType.Element:

                            Node node = new Node { Name = reader.Name };
                            node.Parent = current;

                            if (Root == null)
                                Root = node;

                            if (current != null)
                                current.Nodes.Add(node);

                            if (reader.HasAttributes)
                            {
                                while (reader.MoveToNextAttribute())
                                {
                                    if (reader.Name.Equals("xtype"))
                                        node.Type = reader.Value;
                                    else if (reader.Name.Equals("xkey"))
                                        node.Key = reader.Value;
                                    else if (reader.Name.Equals("xrefid"))
                                        node.RefID = Convert.ToInt32(reader.Value);
                                    else if (reader.Name.Equals("xref"))
                                        node.Reference = Convert.ToInt32(reader.Value);
                                    else
                                    {
                                        Node sub1 = new Node { Name = reader.Name, Value = reader.Value };
                                        node.Nodes.Add(sub1);
                                    }
                                }
                            }

                            current = node;

                            if (reader.IsEmptyElement)
                                current = current.Parent;

                            break;
                        case XmlNodeType.EndElement:
                            if (!reader.IsEmptyElement)
                                current = current.Parent;
                            break;
                        case XmlNodeType.Text:

                            if (!string.IsNullOrEmpty(reader.Value))
                            {
                                string value = reader.Value.Trim();

                                if (!string.IsNullOrEmpty(value))
                                    current.Value = reader.Value;
                            }
                            break;
                        default:
                            break;
                    }
                }
            }
            catch
            {
            }
            return Root;
        }

        private Node FindNode(Node parent, int reference)
        {
            Node found = parent.Nodes.Find(x => x.RefID == reference);
            if (found != null) return found;

            foreach (Node sub in parent.Nodes)
            {
                found = FindNode(sub, reference);
                if (found != null)
                    break;
            }
            return found;
        }

        private object DeserializeTo(object target, Node node, Type propertyType)
        {
            if (node.Reference != 0)
            {
                if (ReadCache.ContainsKey(node.Reference))
                    return ReadCache[node.Reference];
                else
                {
                    Node refnode = FindNode(Root, node.Reference);
                    if (refnode != null)
                        return Deserialize(refnode, propertyType);
                }
            }

            Type type = target.GetType();
            object result = target;
            object value = null;

            if (!(result is IList || result is IDictionary))
            {
                PropertyInfo[] properties = Reflector.GetProperties(type);

                foreach (PropertyInfo info in properties)
                {
                    if (info.IsDefined(typeof(XmlIgnoreAttribute), true)) continue;
                    if (!info.CanWrite) continue;
                    if (info.GetSetMethod() == null) continue;

                    Node child = node.Nodes.Find(x => x.Name.Equals(info.Name));

                    if (child == null) continue;

                    value = null;

                    if (!string.IsNullOrEmpty(child.Value))
                    {
                        TypeConverter converter = System.ComponentModel.TypeDescriptor.GetConverter(info.PropertyType);
                        if (converter != null)
                            value = converter.ConvertFromString(child.Value);
                    }
                    else
                    {
                        try
                        {
                            value = Deserialize(child, info.PropertyType);
                        }
                        catch
                        {

                        }
                    }

                    if (value != null)
                        info.SetValue(result, value, null);
                }
            }

            #region special typeof(Control) case

            if (result is Control)
            {
                Node child;

                if (result is IControlContainer)
                {
                    IControlContainer container = result as IControlContainer;
                    child = node.Nodes.Find(x => x.Name.Equals("Controls"));

                    if (child != null)
                    {
                        for (int i = 0; i < child.Nodes.Count; i++)
                        {
                            value = null;

                            if (container.Controls.Count > i)
                            {
                                DeserializeTo(container.Controls[i], child.Nodes[i], typeof(Control));
                            }
                            else
                            {
                                value = Deserialize(child.Nodes[i], typeof(Control));

                                if (value != null)
                                    container.Controls.Add(value as Control);
                            }
                        }
                    }
                }

                ElementCollection elements = ((Control)result).GetElements();
                child = node.Nodes.Find(x => x.Name.Equals("Elements"));

                if (child != null)
                {
                    for (int i = 0; i < child.Nodes.Count; i++)
                    {
                        DeserializeTo(elements[i], child.Nodes[i], typeof(Control));
                    }
                }
            }

            #endregion

            if (result is IList)
            {
                Type itemType = null;
                if (type.IsGenericType)
                {
                    Type[] gens = type.GetGenericArguments();
                    if (gens.Length > 0)
                        itemType = gens[0];
                }

                foreach (Node child in node.Nodes)
                {
                    ((IList)result).Add(Deserialize(child, itemType));
                }
            }

            if (result is IDictionary)
            {
                Type keyType = null;
                Type itemType = null;

                if (type.IsGenericType)
                {
                    Type[] gens = type.GetGenericArguments();
                    if (gens.Length > 1)
                    {
                        keyType = gens[0];
                        itemType = gens[1];
                    }
                }

                TypeConverter converter = System.ComponentModel.TypeDescriptor.GetConverter(keyType);
                if (converter != null)
                {
                    foreach (Node child in node.Nodes)
                    {
                        object key = converter.ConvertFromString(child.Key);
                        ((IDictionary)result).Add(key, Deserialize(child, itemType));
                    }
                }
            }

            if (node.RefID != 0)
            {
                if (!ReadCache.ContainsKey(node.RefID))
                    ReadCache.Add(node.RefID, result);
            }

            return result;
        }

        private object Deserialize(Node node, Type propertyType)
        {
            if (node.Reference != 0)
            {
                if (ReadCache.ContainsKey(node.Reference))
                    return ReadCache[node.Reference];
                else
                {
                    Node refnode = FindNode(Root, node.Reference);
                    if (refnode != null)
                        return Deserialize(refnode, propertyType);
                }
            }

            Type type = null;

            if (!string.IsNullOrEmpty(node.Type))
            {
                type = Reflector.GetType(node.Type);
            }
            else
                type = propertyType;

            if (type == null)
                return null;

            object result = null;
            object value = null;

            if(type.GetConstructor(new Type[0]{}) != null)
                result = Activator.CreateInstance(type);

            if (!(result is IList || result is IDictionary))
            {
                PropertyInfo[] properties = Reflector.GetProperties(type);

                foreach (PropertyInfo info in properties)
                {
                    value = null;

                    if (info.IsDefined(typeof(XmlIgnoreAttribute), true)) continue;
                    if (!info.CanWrite) continue;
                    if (!info.CanRead) continue;

                    if (info.GetSetMethod() == null) continue;

                    Node child = node.Nodes.Find(x => x.Name.Equals(info.Name));
                    if (child == null) continue;

                    if (!string.IsNullOrEmpty(child.Value))
                    {
                        TypeConverter converter = System.ComponentModel.TypeDescriptor.GetConverter(info.PropertyType);
                        if (converter != null)
                        {
                            if (converter.CanConvertFrom(typeof(string)))
                                value = converter.ConvertFromString(child.Value);
                        }
                    }
                    else
                    {
                        value = Deserialize(child, info.PropertyType);
                    }

                    if (value != null)
                        info.SetValue(result, value, null);
                }
            }

            #region special typeof(Control) case

            if (result is Control)
            {
                Node child;

                if (result is IControlContainer)
                {
                    IControlContainer container = result as IControlContainer;
                    child = node.Nodes.Find(x => x.Name.Equals("Controls"));

                    if (child != null)
                    {
                        for (int i = 0; i < child.Nodes.Count; i++)
                        {
                            if (container.Controls.Count > i)
                            {
                                DeserializeTo(container.Controls[i], child.Nodes[i], typeof(Control));
                            }
                            else
                            {
                                value = Deserialize(child.Nodes[i], typeof(Control));

                                if (value != null)
                                    container.Controls.Add(value as Control);
                            }
                        }
                    }
                }

                ElementCollection elements = ((Control)result).GetElements();
                child = node.Nodes.Find(x => x.Name.Equals("Elements"));

                if (child != null)
                {
                    for (int i = 0; i < child.Nodes.Count; i++)
                    {
                        DeserializeTo(elements[i], child.Nodes[i], typeof(Control));
                    }
                }
            }

            #endregion

            if (result is IList)
            {
                Type itemType = null;
                if (type.IsGenericType)
                {
                    Type[] gens = type.GetGenericArguments();
                    if (gens.Length > 0)
                        itemType = gens[0];
                }

                foreach (Node child in node.Nodes)
                {
                    ((IList)result).Add(Deserialize(child, itemType));
                }
            }

            if (result is IDictionary)
            {
                Type keyType = null;
                Type itemType = null;

                if (type.IsGenericType)
                {
                    Type[] gens = type.GetGenericArguments();
                    if (gens.Length > 1)
                    {
                        keyType = gens[0];
                        itemType = gens[1];
                    }
                }
                else if (type.BaseType.IsGenericType)
                {
                    Type[] gens = type.BaseType.GetGenericArguments();
                    if (gens.Length > 1)
                    {
                        keyType = gens[0];
                        itemType = gens[1];
                    }
                }

                TypeConverter converter = System.ComponentModel.TypeDescriptor.GetConverter(keyType);
                if (converter != null)
                {
                    foreach (Node child in node.Nodes)
                    {
                        object key = converter.ConvertFromString(child.Key);
                        ((IDictionary)result).Add(key, Deserialize(child, itemType));
                    }
                }
            }

            if (node.RefID != 0)
            {
                if (!ReadCache.ContainsKey(node.RefID))
                    ReadCache.Add(node.RefID, result);
            }

            return result;
        }

        #region IDisposable Members

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposed)
        {
        }

        #endregion
    }

}

