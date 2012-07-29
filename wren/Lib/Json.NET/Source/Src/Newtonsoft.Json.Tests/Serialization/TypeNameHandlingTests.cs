﻿#region License
// Copyright (c) 2007 James Newton-King
//
// Permission is hereby granted, free of charge, to any person
// obtaining a copy of this software and associated documentation
// files (the "Software"), to deal in the Software without
// restriction, including without limitation the rights to use,
// copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the
// Software is furnished to do so, subject to the following
// conditions:
//
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
// OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
// HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
// WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
// OTHER DEALINGS IN THE SOFTWARE.
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization.Formatters;
using System.Text;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Tests.TestObjects;
using NUnit.Framework;
using Newtonsoft.Json.Utilities;
using System.Net;

namespace Newtonsoft.Json.Tests.Serialization
{
  public class TypeNameHandlingTests : TestFixtureBase
  {
    [Test]
    public void WriteTypeNameForObjects()
    {
      string employeeRef = ReflectionUtils.GetTypeName(typeof(EmployeeReference), FormatterAssemblyStyle.Simple);

      EmployeeReference employee = new EmployeeReference();

      string json = JsonConvert.SerializeObject(employee, Formatting.Indented, new JsonSerializerSettings
      {
        TypeNameHandling = TypeNameHandling.Objects
      });

      Assert.AreEqual(@"{
  ""$id"": ""1"",
  ""$type"": """ + employeeRef + @""",
  ""Name"": null,
  ""Manager"": null
}", json);
    }

    [Test]
    public void DeserializeTypeName()
    {
      string employeeRef = ReflectionUtils.GetTypeName(typeof(EmployeeReference), FormatterAssemblyStyle.Simple);

      string json = @"{
  ""$id"": ""1"",
  ""$type"": """ + employeeRef + @""",
  ""Name"": ""Name!"",
  ""Manager"": null
}";

      object employee = JsonConvert.DeserializeObject(json, null, new JsonSerializerSettings
      {
        TypeNameHandling = TypeNameHandling.Objects
      });

      Assert.IsInstanceOfType(typeof(EmployeeReference), employee);
      Assert.AreEqual("Name!", ((EmployeeReference)employee).Name);
    }

#if !SILVERLIGHT && !PocketPC
    [Test]
    public void DeserializeTypeNameFromGacAssembly()
    {
      string cookieRef = ReflectionUtils.GetTypeName(typeof(Cookie), FormatterAssemblyStyle.Simple);

      string json = @"{
  ""$id"": ""1"",
  ""$type"": """ + cookieRef + @"""
}";

      object cookie = JsonConvert.DeserializeObject(json, null, new JsonSerializerSettings
      {
        TypeNameHandling = TypeNameHandling.Objects
      });

      Assert.IsInstanceOfType(typeof(Cookie), cookie);
    }
#endif

    [Test]
    public void SerializeGenericObjectListWithTypeName()
    {
      string employeeRef = typeof(EmployeeReference).AssemblyQualifiedName;
      string personRef = typeof(Person).AssemblyQualifiedName;

      List<object> values = new List<object>
        {
          new EmployeeReference
            {
              Name = "Bob",
              Manager = new EmployeeReference {Name = "Frank"}
            },
          new Person
            {
              Department = "Department",
              BirthDate = new DateTime(2000, 12, 30, 0, 0, 0, DateTimeKind.Utc),
              LastModified = new DateTime(2000, 12, 30, 0, 0, 0, DateTimeKind.Utc)
            },
          "String!",
          int.MinValue
        };

      string json = JsonConvert.SerializeObject(values, Formatting.Indented, new JsonSerializerSettings
      {
        TypeNameHandling = TypeNameHandling.Objects,
        TypeNameAssemblyFormat = FormatterAssemblyStyle.Full
      });

      Assert.AreEqual(@"[
  {
    ""$id"": ""1"",
    ""$type"": """ + employeeRef + @""",
    ""Name"": ""Bob"",
    ""Manager"": {
      ""$id"": ""2"",
      ""$type"": """ + employeeRef + @""",
      ""Name"": ""Frank"",
      ""Manager"": null
    }
  },
  {
    ""$type"": """ + personRef + @""",
    ""Name"": null,
    ""BirthDate"": ""\/Date(978134400000)\/"",
    ""LastModified"": ""\/Date(978134400000)\/""
  },
  ""String!"",
  -2147483648
]", json);
    }

    [Test]
    public void DeserializeGenericObjectListWithTypeName()
    {
      string employeeRef = typeof(EmployeeReference).AssemblyQualifiedName;
      string personRef = typeof(Person).AssemblyQualifiedName;

      string json = @"[
  {
    ""$id"": ""1"",
    ""$type"": """ + employeeRef + @""",
    ""Name"": ""Bob"",
    ""Manager"": {
      ""$id"": ""2"",
      ""$type"": """ + employeeRef + @""",
      ""Name"": ""Frank"",
      ""Manager"": null
    }
  },
  {
    ""$type"": """ + personRef + @""",
    ""Name"": null,
    ""BirthDate"": ""\/Date(978134400000)\/"",
    ""LastModified"": ""\/Date(978134400000)\/""
  },
  ""String!"",
  -2147483648
]";

      List<object> values = (List<object>)JsonConvert.DeserializeObject(json, typeof(List<object>), new JsonSerializerSettings
      {
        TypeNameHandling = TypeNameHandling.Objects,
        TypeNameAssemblyFormat = FormatterAssemblyStyle.Full
      });

      Assert.AreEqual(4, values.Count);

      EmployeeReference e = (EmployeeReference)values[0];
      Person p = (Person)values[1];

      Assert.AreEqual("Bob", e.Name);
      Assert.AreEqual("Frank", e.Manager.Name);

      Assert.AreEqual(null, p.Name);
      Assert.AreEqual(new DateTime(2000, 12, 30, 0, 0, 0, DateTimeKind.Utc), p.BirthDate);
      Assert.AreEqual(new DateTime(2000, 12, 30, 0, 0, 0, DateTimeKind.Utc), p.LastModified);

      Assert.AreEqual("String!", values[2]);
      Assert.AreEqual(int.MinValue, values[3]);
    }

    [Test]
    [ExpectedException(typeof(JsonSerializationException))]
    public void DeserializeWithBadTypeName()
    {
      string employeeRef = typeof(EmployeeReference).AssemblyQualifiedName;

      string json = @"{
  ""$id"": ""1"",
  ""$type"": """ + employeeRef + @""",
  ""Name"": ""Name!"",
  ""Manager"": null
}";

      JsonConvert.DeserializeObject(json, typeof(Person), new JsonSerializerSettings
      {
        TypeNameHandling = TypeNameHandling.Objects,
        TypeNameAssemblyFormat = FormatterAssemblyStyle.Full
      });
    }

    [Test]
    public void DeserializeTypeNameWithNoTypeNameHandling()
    {
      string employeeRef = typeof(EmployeeReference).AssemblyQualifiedName;

      string json = @"{
  ""$id"": ""1"",
  ""$type"": """ + employeeRef + @""",
  ""Name"": ""Name!"",
  ""Manager"": null
}";

      JObject o = (JObject)JsonConvert.DeserializeObject(json);

      Assert.AreEqual(@"{
  ""Name"": ""Name!"",
  ""Manager"": null
}", o.ToString());
    }

    [Test]
    [ExpectedException(typeof(JsonSerializationException), ExpectedMessage = "Type specified in JSON 'Newtonsoft.Json.Tests.TestObjects.Employee' was not resolved.")]
    public void DeserializeTypeNameOnly()
    {
      string json = @"{
  ""$id"": ""1"",
  ""$type"": ""Newtonsoft.Json.Tests.TestObjects.Employee"",
  ""Name"": ""Name!"",
  ""Manager"": null
}";

      JsonConvert.DeserializeObject(json, null, new JsonSerializerSettings
      {
        TypeNameHandling = TypeNameHandling.Objects
      });
    }

    public interface ICorrelatedMessage
    {
      string CorrelationId { get; set; }
    }

    public class SendHttpRequest : ICorrelatedMessage
    {
      public SendHttpRequest()
      {
        RequestEncoding = "UTF-8";
        Method = "GET";
      }
      public string Method { get; set; }
      public Dictionary<string, string> Headers { get; set; }
      public string Url { get; set; }
      public Dictionary<string, string> RequestData;
      public string RequestBodyText { get; set; }
      public string User { get; set; }
      public string Passwd { get; set; }
      public string RequestEncoding { get; set; }
      public string CorrelationId { get; set; }
    }

    [Test]
    public void DeserializeGenericTypeName()
    {
      string typeName = typeof(SendHttpRequest).AssemblyQualifiedName;

      string json = @"{
""$type"": """ + typeName + @""",
""RequestData"": {
""$type"": ""System.Collections.Generic.Dictionary`2[[System.String, mscorlib,Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089],[System.String, mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]], mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089"",
""Id"": ""siedemnaście"",
""X"": ""323""
},
""Method"": ""GET"",
""Url"": ""http://www.onet.pl"",
""RequestEncoding"": ""UTF-8"",
""CorrelationId"": ""xyz""
}";

      ICorrelatedMessage message = JsonConvert.DeserializeObject<ICorrelatedMessage>(json, new JsonSerializerSettings
        {
          TypeNameHandling = TypeNameHandling.Objects,
          TypeNameAssemblyFormat = FormatterAssemblyStyle.Full
        });

      Assert.IsInstanceOfType(typeof(SendHttpRequest), message);

      SendHttpRequest request = (SendHttpRequest)message;
      Assert.AreEqual("xyz", request.CorrelationId);
      Assert.AreEqual(2, request.RequestData.Count);
      Assert.AreEqual("siedemnaście", request.RequestData["Id"]);
    }

    [Test]
    public void SerializeObjectWithMultipleGenericLists()
    {
      string containerTypeName = typeof(Container).AssemblyQualifiedName;
      string productTypeName = typeof(Product).AssemblyQualifiedName;

      Container container = new Container
                          {
                            In = new List<Product>(),
                            Out = new List<Product>()
                          };

      string json = JsonConvert.SerializeObject(container, Formatting.Indented,
          new JsonSerializerSettings
              {
                NullValueHandling = NullValueHandling.Ignore,
                TypeNameHandling = TypeNameHandling.All,
                TypeNameAssemblyFormat = FormatterAssemblyStyle.Full
              });

      Assert.AreEqual(@"{
  ""$type"": """ + containerTypeName + @""",
  ""In"": {
    ""$type"": ""System.Collections.Generic.List`1[[" + productTypeName + @"]], mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089"",
    ""$values"": []
  },
  ""Out"": {
    ""$type"": ""System.Collections.Generic.List`1[[" + productTypeName + @"]], mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089"",
    ""$values"": []
  }
}", json);
    }

    public class TypeNameProperty
    {
      public string Name { get; set; }
      [JsonProperty(TypeNameHandling = TypeNameHandling.All)]
      public object Value { get; set; }
    }

    [Test]
    public void WriteObjectTypeNameForProperty()
    {
      string typeNamePropertyRef = ReflectionUtils.GetTypeName(typeof(TypeNameProperty), FormatterAssemblyStyle.Simple);

      TypeNameProperty typeNameProperty = new TypeNameProperty
                                            {
                                              Name = "Name!",
                                              Value = new TypeNameProperty
                                                        {
                                                          Name = "Nested!"
                                                        }
                                            };

      string json = JsonConvert.SerializeObject(typeNameProperty, Formatting.Indented);

      Assert.AreEqual(@"{
  ""Name"": ""Name!"",
  ""Value"": {
    ""$type"": """ + typeNamePropertyRef + @""",
    ""Name"": ""Nested!"",
    ""Value"": null
  }
}", json);

      TypeNameProperty deserialized = JsonConvert.DeserializeObject<TypeNameProperty>(json);
      Assert.AreEqual("Name!", deserialized.Name);
      Assert.IsInstanceOfType(typeof(TypeNameProperty), deserialized.Value);

      TypeNameProperty nested = (TypeNameProperty)deserialized.Value;
      Assert.AreEqual("Nested!", nested.Name);
      Assert.AreEqual(null, nested.Value);
    }

    [Test]
    public void WriteListTypeNameForProperty()
    {
      string listRef = ReflectionUtils.GetTypeName(typeof(List<int>), FormatterAssemblyStyle.Simple);

      TypeNameProperty typeNameProperty = new TypeNameProperty
      {
        Name = "Name!",
        Value = new List<int> { 1, 2, 3, 4, 5 }
      };

      string json = JsonConvert.SerializeObject(typeNameProperty, Formatting.Indented);

      Assert.AreEqual(@"{
  ""Name"": ""Name!"",
  ""Value"": {
    ""$type"": """ + listRef + @""",
    ""$values"": [
      1,
      2,
      3,
      4,
      5
    ]
  }
}", json);

      TypeNameProperty deserialized = JsonConvert.DeserializeObject<TypeNameProperty>(json);
      Assert.AreEqual("Name!", deserialized.Name);
      Assert.IsInstanceOfType(typeof(List<int>), deserialized.Value);

      List<int> nested = (List<int>)deserialized.Value;
      Assert.AreEqual(5, nested.Count);
      Assert.AreEqual(1, nested[0]);
      Assert.AreEqual(2, nested[1]);
      Assert.AreEqual(3, nested[2]);
      Assert.AreEqual(4, nested[3]);
      Assert.AreEqual(5, nested[4]);
    }
  }
}