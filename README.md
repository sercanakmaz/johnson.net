johnson.net - a simple .net project helper
===========

Features
--------
1. Environment configuration operations
2. Convert operations
3. Data operations
4. IO operations
5. Log operations
6. Mail operations
7. Multithread operations
8. Reflection operations


1. Environment Configuration
------------------------------------------------------------

1 Add config section to your web.config/appconfig

```xml
<configSections>
    <section name="environmentConfig" type="JohnsonNet.Config.EnvironmentConfig,JohnsonNet"/>
</configSections>
```

2 Add environment rule (ComputerName)

```xml
<environmentConfig live="~/Config/Live.config" local="~/Config/Local.config" test="~/Config/Test.config" provider="JohnsonNet.Config.ConfigurationFileProvider">
    <rules type="ComputerName">
      <add environment="Local" param="LocalComputerName"/>
      <add environment="Test" param="TestComputerName"/>
      <add environment="Live" param="LiveComputerName"/>
    </rules>
</environmentConfig>
```

3 Add environment rule (RequestHost)
```xml
  <environmentConfig live="~/Config/Live.config" local="~/Config/Local.config" test="~/Config/Test.config" provider="JohnsonNet.Config.ConfigurationFileProvider">
    <rules type="Request">
      <add environment="Test" param="test.johnson.net"/>
      <add environment="Live" param="johnson.net"/>
      <add environment="Live" param="www.johnson.net"/>
    </rules>
  </environmentConfig>
```

4 Add environment configuration files to your visual studio project
5 Get your configuration data.

```csharp
    var connectionString = JohnsonManager.Config.Current.GetConnectionString("LocalSqlServer");
    var applicationID = JohnsonManager.Config.Current.GetSetting<long>("Facebook-ApplicationID");
    var applicationGuid = JohnsonManager.Config.Current.GetSetting<Guid>("Facebook-Guid");
    var service = JohnsonManager.Config.Current.GetCommunicationObject<IYourServiceChannel>();
```
Also 

1 You can get your current environment.
```csharp
JohnsonManager.Config.CurrentEnvironment // Local,Test,PreProduction Live
```
2 You can build your own ConfigurationProvider.
```xml
  <environmentConfig provider="YourNameSpace.YourConfigurationProvider">
  
  </environmentConfig>
```

2. Convert anything to anything
------------------------------------------------------------
```csharp
var nullableGuid = JohnsonManager.Convert.To<Guid?>("12312sfd");
var yourEnum = JohnsonManager.Convert.To<YourEnum>("En");
var yourDecimal = JohnsonManager.Convert.To(typeof(decimal), "123.12");
var yourDouble = JohnsonManager.Convert.To<double>("123.12");
```
3. A simple ORM
```csharp
```
