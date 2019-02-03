# FastMapper
![Image of Yaktocat](https://ci.appveyor.com/api/projects/status/dxqgnl1kfbabenv2?svg=true)

Allows mapping between two objects.  Based on FastMember library.
```
dynamic result = DataProvider.QueryDynamic(select).FirstOrDefault();
return ObjectMapper.Map<ViewModel>(result);
```
Configuration can be done for abstract types:
```
ObjectMapper.Init(config =>
{
  config.Add<ViewModel>(targetConfig => targetConfig.For(x => x.Address).ResolveWith<Address>());
});
```
Configuration can also be done inline when mapping:
```
SourceProperty data = new SourceProperty
{
  PostCode = "nw10 7nz",
};

Property property = ObjectMapper.Map<SourceProperty, Property>(data, config =>
{
  config.For(x => x.MapFromTest).MapFrom(x => x.PostCode);
});

property.MapFromTest.MustEqual(data.PostCode);
```
