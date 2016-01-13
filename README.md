# johnson.net - a simple .net project helper
===========

## Features
--------
1. Environment configuration operations
2. Convert operations
3. Data operations
4. IO operations
5. Log operations
6. Mail operations
7. Multithread operations
8. Reflection operations


## 1. Environment Configuration
------------------------------------------------------------

###### Step 1 Add config section to your web.config/appconfig

```xml
<configSections>
    <section name="environmentConfig" type="JohnsonNet.Config.EnvironmentConfig,JohnsonNet"/>
</configSections>
```

##### Step 2 Add Environment Rule 

###### ComputerName

```xml
<environmentConfig live="~/Config/Live.config" local="~/Config/Local.config" test="~/Config/Test.config" provider="JohnsonNet.Config.ConfigurationFileProvider">
    <rules type="ComputerName">
      <add environment="Local" param="LocalComputerName"/>
      <add environment="Test" param="TestComputerName"/>
      <add environment="Live" param="LiveComputerName"/>
    </rules>
</environmentConfig>
```

###### RequestHost

```xml
  <environmentConfig live="~/Config/Live.config" local="~/Config/Local.config" test="~/Config/Test.config" provider="JohnsonNet.Config.ConfigurationFileProvider">
    <rules type="Request">
      <add environment="Test" param="test.johnson.net"/>
      <add environment="Live" param="johnson.net"/>
      <add environment="Live" param="www.johnson.net"/>
    </rules>
  </environmentConfig>
```

#### Step 4 Add environment configuration files to your visual studio project

![alt tag](https://raw.githubusercontent.com/srjohn/johnson.net/master/ReadMeAssets/solution-explorer-config.png)

#### Step 5 Get your configuration data.

```csharp
    var connectionString = JohnsonManager.Config.Current.GetConnectionString("LocalSqlServer");
    var applicationID = JohnsonManager.Config.Current.GetSetting<long>("Facebook-ApplicationID");
    var applicationGuid = JohnsonManager.Config.Current.GetSetting<Guid>("Facebook-Guid");
    var service = JohnsonManager.Config.Current.GetCommunicationObject<IYourServiceChannel>();
```
#### Also 

##### You can get your current environment.
```csharp
JohnsonManager.Config.CurrentEnvironment // Local,Test,PreProduction Live
```
##### You can build your own ConfigurationProvider.
```xml
  <environmentConfig provider="YourNameSpace.YourConfigurationProvider">
  
  </environmentConfig>
```

## 2. Convert anything to anything
------------------------------------------------------------
```csharp
var nullableGuid = JohnsonManager.Convert.To<Guid?>("12312sfd");
var yourEnum = JohnsonManager.Convert.To<YourEnum>("En");
var yourDecimal = JohnsonManager.Convert.To(typeof(decimal), "123.12");
var yourDouble = JohnsonManager.Convert.To<double>("123.12");
```

## 3. A simple ORM
------------------------------------------------------------

It's looks simple but in a optimum level, it has all you need to develop a enterprise application.

By default JohnsonManager.Data uses "LocalSqlServer" connection string, but you can get a new instance from JohnsonNet.Operation.DataOperation class to use your own connection string.

#### Execute Method

```csharp
class Product
{
    public int ID { get; set; }
    public string Name { get; set; }
}
class MappedProduct
{
    [FieldMap("column_product_id")]
    public int ProductID { get; set; }
    [FieldMap("column_product_name")]
    public string ProductName { get; set; }
}

JohnsonManager.Data.Execute<Product>("GetProduct", new ParamDictionary
{
    { "ID", 1 }
});
```
```csharp FieldMap ``` attribute can help you rename your database field.

JohnsonManager.Data supports multiple resultsets from your database.

```csharp
var resulSets = JohnsonManager.Data.Execute<Product,MappedProduct>("GetProduct", new ParamDictionary
{
    { "ID", 1 }
});

var products = resultSets.Result1;
var mappedProducts = resultSets.Result2;
```

#### ExecuteReader method

```csharp
JohnsonManager.Data.ExecuteReader("dbo.GetProduct", new ParamDictionary
{
    { "ID", 1 }
}, (reader) =>
{
    while (reader.Read())
    {

    }
});
```
