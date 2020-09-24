using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;

namespace FastMapper.UnitTest
{
  [TestClass]
  public class ObjectMapperTests
  {
    [TestInitialize]
    public void TestInitialize()
    {
      ObjectMapper.Init(config =>
      {
        config.Add<Property>(targetConfig => targetConfig.For(x => x.Address).ResolveWith<Address>());
      });
    }

    [TestCleanup]
    public void TestCleanup()
    {
      Configuration.Current.Clear();
    }

    [TestMethod]
    public void dynamic_speed_test()
    {
      dynamic data = new ExpandoObject();

      data.Id = 1;
      data.Address01 = "Address01";
      data.PostCode = "PostCode";
      data.NotThis = "ignore";

      Profiler.All("map", 100000, () => ObjectMapper.Map<Property>(data));
    }

    [TestMethod]
    public void maps_to_dynamic_object()
    {
      dynamic data = new ExpandoObject();

      data.Id = 1;
      data.Address01 = "Address01";
      data.PostCode = "PostCode";
      data.NotThese = "PostCode";

      Property property = ObjectMapper.Map<Property>(data);

      property.Id.MustEqual(1);
      property.Address.Address01.MustEqual("Address01");
      property.Address.PostCode.MustEqual("PostCode");
    }

    [TestMethod]
    public void maps_dynamic_string_to_int_property()
    {
      dynamic data = new ExpandoObject();

      data.Id = "1";

      Property property = ObjectMapper.Map<Property>(data);

      property.Id.MustEqual(1);
    }

    [TestMethod]
    public void maps_from_anon_object()
    {
      object data = new
      {
        Id = 5,
        PostCode = "tn2 4hp",
      };

      Property property = ObjectMapper.Map<Property>(data);

      property.Id.MustEqual(5);
      property.Address.PostCode.MustEqual("tn2 4hp");
    }

    [TestMethod]
    public void maps_from_class()
    {
      SourceProperty data = new SourceProperty();

      Property property = ObjectMapper.Map<Property>(data);

      property.Id.MustEqual(data.Id);
      property.Address.PostCode.MustEqual(data.PostCode);
    }

    [TestMethod]
    public void maps_date()
    {
      DateTime now = DateTime.Now;

      object data = new
      {
        Now = now,
      };

      Property property = ObjectMapper.Map<Property>(data);

      property.Now.MustEqual(now);
    }

    [TestMethod]
    public void maps_enum()
    {
      object data = new
      {
        PropertyType = 2,
      };

      Property property = ObjectMapper.Map<Property>(data);

      property.PropertyType.MustEqual(Types.Type2);
    }

    [TestMethod]
    public void maps_nullable_int()
    {
      object data = new
      {
        NullableInt = 23,
      };

      Property property = ObjectMapper.Map<Property>(data);

      if(property.NullableInt != 23)
      {
        string d = "";
      }

      property.NullableInt.MustEqual(23);
    }

    [TestMethod]
    public void map_allows_configuration()
    {
      SourceProperty data = new SourceProperty
      {
        PostCode = "bn26 6ep",
      };

      Property property = ObjectMapper.Map<SourceProperty, Property>(data, config =>
      {
        config.For(x => x.MapFromTest).MapFrom(x => x.PostCode);
      });

      property.MapFromTest.MustEqual(data.PostCode);
    }

    [TestMethod]
    public void map_allows_dynamic_configuration()
    {
      var data = new
      {
        PostCode = "bn26 6ep",
      };

      SourceProperty property = ObjectMapper.Map<dynamic, SourceProperty>(data, config =>
      {
        config.For(x => x.FileName).MapFrom("PostCode");
      });

      property.FileName.MustEqual("bn26 6ep");
    }

    [TestMethod]
    public void map_supports_enumerable()
    {
      SourceProperty data = new SourceProperty
      {
        PostCode = "bn26 6ep",
        FileName = "test.jpg",
      };

      Property property = ObjectMapper.Map<SourceProperty, Property>(data, config =>
      {
        config.ForEach(x => x.Files).ResolveWith<File>();
      });

      property.Files.Count().MustEqual(1);
      property.Files.First().FileName.MustEqual(data.FileName);
    }

    [TestMethod]
    public void map_supports_enumerable_with_resolver()
    {
      SourceProperty data = new SourceProperty
      {
        PostCode = "bn26 6ep",
        FileName = "test.jpg",
      };

      Property property = ObjectMapper.Map<SourceProperty, Property>(data, config =>
      {
        config.ForEach(x => x.AbstractFiles).ResolveWith<File>();
      });

      property.AbstractFiles.Count().MustEqual(1);
      property.AbstractFiles.First().FileName.MustEqual(data.FileName);
    }

    [TestMethod]
    public void maps_to_different_property()
    {
      SourceProperty data = new SourceProperty
      {
        Id = 9
      };

      Property property = ObjectMapper.Map<SourceProperty, Property>(data, config =>
      {
        config.For(x => x.NullableInt).MapFrom(x => x.Id);
      });

      property.NullableInt.MustEqual(data.Id);
    }

    [TestMethod]
    public void map_converts_from_string()
    {
      dynamic data = new
      {
        Id = "9"
      };

      Property property = ObjectMapper.Map<Property>(data);

      property.Id.MustEqual(9);
    }

    [TestMethod]
    public void mapall_maps_all_items()
    {
      dynamic data = new[] {
        new
        {
          Id = 1
        },
        new
        {
          Id = 2
        },
      };

      IEnumerable<Property> properties = ObjectMapper.MapAll<Property>(data);

      properties.Count().MustEqual(2);
      properties.First().Id.MustEqual(1);
      properties.Skip(1).First().Id.MustEqual(2);
    }

    [TestMethod]
    public void mapall_maps_all_items_allowing_configuration()
    {
      var data = new[] {
        new
        {
          Id = 4
        },
        new
        {
          Id = 5
        },
      };

      IEnumerable<Property> properties = ObjectMapper.MapAll<dynamic, Property>(data, config =>
      {
        config.For(x => x.NullableInt).MapFrom("Id");
      });

      properties.Count().MustEqual(2);
      properties.First().NullableInt.MustEqual(4);
      properties.Skip(1).First().NullableInt.MustEqual(5);
    }

    /// <summary>
    /// This test a situation when you have a property with a class and the mapper will create and map that child property with values from the incoming source.
    /// </summary>
    [TestMethod]
    public void Map_maps_properties_of_complex_type_from_source_values()
    {
      dynamic data = new
      {
        Name = "bob",
        Address01 = "address-01",
        PostCode = "post-code"
      };

      Person person = ObjectMapper.Map<Person>(data);

      person.Name.MustEqual("bob");
      person.Address.Address01.MustEqual("address-01");
      person.Address.PostCode.MustEqual("post-code");
    }

    [TestMethod]
    public void works_with_fields()
    {
      dynamic data = new
      {
        Name = "bob",
        Address01 = "address-01",
        PostCode = "post-code",
        WritableField = "field-value"
      };

      PersonWithField person = ObjectMapper.Map<PersonWithField>(data);

      person.Name.MustEqual("bob");
      person.Address.Address01.MustEqual("address-01");
      person.Address.PostCode.MustEqual("post-code");
      person.WritableField.MustEqual("field-value");
    }

    [TestMethod]
    public void works_with_member_as_array()
    {
      dynamic data = new
      {
        Title = "a-title",
        Address01 = "address-01",
        PostCode = "PostCode-01"
      };

      TypeWithArray testInstance = ObjectMapper.Map<TypeWithArray>(data);

      testInstance.Title.MustEqual("a-title");
    }

    /// <summary>
    /// This checks for an issue where we try to map to a primative list i.e. List<string>.
    /// </summary>
    [TestMethod]
    public void primative_list_does_not_throw()
    {
      ObjectMapper.Map<TestWithPrimativeList>(new
      {
        Title = "a-title",
        Address01 = "address-01",
        PostCode = "PostCode-01"
      });
    }

    [TestMethod]
    public void nested_or_circulare_references()
    {
      Office office = ObjectMapper.Map<Office>(new
      {
        OfficeName = "office-1",
        BranchName = "branch-1"
      });

      var f = office;
    }
  }

  public enum Types
  {
    None = 0,
    Type1 = 1,
    Type2 = 2,
  }

  public class SourceProperty
  {
    public int Id { get; set; }

    public string PostCode { get; set; }

    public int PropertyType { get; set; }

    public string FileName { get; set; }
  }

  public class File : IFile
  {
    public string FileName { get; set; }
  }

  public interface IFile
  {
    string FileName { get; set; }
  }

  public class Property
  {
    public int Id { get; set; }

    public IAddress Address { get; set; }

    public DateTime Now { get; set; }

    public Types PropertyType { get; set; }

    public int? NullableInt { get; set; }

    public string MapFromTest { get; set; }

    public IEnumerable<File> Files { get; set; }

    public IEnumerable<IFile> AbstractFiles { get; set; }
  }

  public class Person
  {
    public string Name { get; set; }

    public Address Address { get; set; }
  }

  public class PersonWithField : Person
  {
    public string WritableField;
  }

  public class Address : IAddress
  {
    public string Address01 { get; set; }

    public string PostCode { get; set; }
  }

  public interface IAddress
  {
    string Address01 { get; set; }

    string PostCode { get; set; }
  }

  public class TypeWithArray
  {
    public string Title { get; set; }

    public Address[] Addresses { get; set; }
  }

  public class TestWithPrimativeList
  {
    public IList<string> Notes { get; set; }
  }

  public class Branch
  {
    public string BranchName;
    public Office Office;
  }

  public class Office
  {
    public string OfficeName;
    public Branch Branch;
  }
}