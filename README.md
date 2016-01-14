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

## 1. Environment Configuration
------------------------------------------------------------

#### Installing

##### Step 1: Add config section to your web.config/appconfig

```xml
<configSections>
    <section name="environmentConfig" type="JohnsonNet.Config.EnvironmentConfig,JohnsonNet"/>
</configSections>
```

##### Step 2: Add Environment Rule 

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

##### Step 4: Add environment configuration files to your visual studio project

![alt tag](https://raw.githubusercontent.com/srjohn/johnson.net/master/ReadMeAssets/solution-explorer-config.png)

##### Step 5: Get your configuration data.

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

### JohnsonNet.Operation.DataOperation class

#### Execute Method

Execute method has 2 argument, procedure name and parameters. And it will convert database results to a entity list.

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
```FieldMap ``` attribute can help you rename your database field.

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

ExecuteReader method has 3 argument, first two procedure name and parameters. Third is a Action<IDataReader>, this allows you to get a your results with IDataRedeader.

I'm using this library in all my projects. And i used Execute and other method a lot. But used ExecuteReader method once. In that case; i had a stored procedure with dymamic pivot table. Of couse didn't know how many columns coming from database. You can see my solution to this problem. ExecuteReader saved my life for this scenario.

```csharp
public class SaleResult : Dictionary<string,object>
{

}
    
SebDataRepository.Cms.ExecuteReader("Report.Sale", parameters, (reader) =>
{
    while (reader.Read())
    {
        Entities.Report.SaleResult row = new Entities.Report.SaleResult();
        for (int i = 0; i < reader.FieldCount; i++)
        {
            row.Add(reader.GetName(i), reader[i]);
        }
        result.Add(row);
    }
});
```

Of course you can use whatever you like, basic sample is below.

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
#### ExecuteBulk method

SQL Server Managament Studio has a keyword named "GO", this keyword allows you to run several scripts in one file. Sometimes we need this on C#, this method helps you to that.

```csharp
JohnsonManager.Data.ExecuteBulk("ALTER TABLE dbo.Product ADD Price INT\r\nGO\r\nALTER TABLE dbo.Product ADD Wat INT")
```

#### ExecuteNonQuery method

You know this allready.

### JohnsonNet.Data.Extensions class

#### ToParamDictionary extension method

```csharp
public static ParamDictionary ToParamDictionary(this object obj)
```

This method allows you to prepare a dictionary list with your entity. You can use this dictionary to send data to your database. And of cours its considers Ignore and FieldMap attributes. If a property has a Ignore attribute, it will not show up in the dictionary. Or if a property has a FieldMap attribute it will show up with mapped name.

#### ToList extension method

It can convert your IDataReader to a entity list with considering ```FieldMap``` attribute.

```csharp
public static List<T> ToList<T>(this IDataReader reader)
```

#### GetFieldOrdinal extension method

You can get columns ordinal in a ```IDataRecord```.

```csharp
public static int GetFieldOrdinal(this IDataRecord dr, string columnName)
```

#### ToConnectionStringSettings extension method

You can get ConnectionStringSettings object from a IDbConnection

```csharp
public static ConnectionStringSettings ToConnectionStringSettings(this IDbConnection connection)
```

#### ToIDbConnection extension method

You can get IDbConnection object from a ConnectionStringSettings

```csharp
public static IDbConnection ToIDbConnection(this ConnectionStringSettings setting)
```

### JohnsonNet.Operation.IOOperation class

#### ToFriendlySizeString method

It will return byte count a friendly text. Like, 1 GB 121 MB

```csharp
public string ToFriendlySizeString(long bytes)
```

#### NextAvailableFilename method

This method allows you to get next available filename in the folder. If a file exists with same time, method will add a suffix like "(1)".

```csharp
public string NextAvailableFilename(string path)
```

#### RemoveInvalidFileNameChars method

It will remove invalid chars from your file name.

```csharp
public string RemoveInvalidFileNameChars(string path)
```

#### ToSEOFriendlyFileName method

It will convert your filename to a SEO friendly status.

```csharp
public string ToSEOFriendlyFileName(string fileName)
```

#### GetResourceStream method

This method allows you to get a method with string

```csharp
public string GetResourceStream(Assembly assembly, string resourceName)
```

### 5. JohnsonNet.Operation.LogOperation class

JohnsonNet has a logging mechanism too. I have to admit, this is amateur. I didn't have time to enhance this. But it will do the job.

#### Configuring

##### Step 1: Add config section to your web.config/appconfig

```xml
<appSettings>
    <add key="LogProjectName" value="YourProjectName" />
    <add key="LogType" value="SaveDatabase;SendMail" />
    
    <add key="LogSmtpUser" value="" />
    <add key="LogSmtpPass" value="" />
    <add key="LogSmtpPort" value="" />
    <add key="LogSmtpServer" value="" />
    <add key="LogSmtpEnableSSL" value="True" />
    
</appSettings>
```

##### Step 2: Define your SaveDatabaseAction.

If your LogType has SaveDatabase, johnsonnet will run this method with your parameters. You can specify this method in global application start, in windows start of main method.

```csharp
JohnsonManager.Logger.SaveDatabaseAction = (DateTime date, string exception, string extra, string project) =>
{
    JohnsonManager.Data.ExecuteNonQuery("System.SaveLog", new ParamDictionary
    {
        { "Date", date },
        { "Exception", exception },
        { "Extra", extra },
        { "Project", project },
    });
};
```

#### Step 3: Using

JohnsonManager.Logger.Log

##### Log Method Usage

This method has 9 overriding method. But it has 2 point of view.

###### Point of View 1: Sending Exception class as Log.

In this point of view, you can send ```Exception``` class as parameter. And send your parameters to cause exception as extra. Sample is below

```csharp
static void SaveProduct(Product product)
{
    try
    {
        ParamDictionary parameters = product.ToParamDictionary();
        JohnsonManager.Data.ExecuteNonQuery("dbo.SaveProduct", parameters);
    }
    catch (Exception ex)
    {
        JohnsonManager.Logger.Log(ex, "ProductID: {0}, ProductName: {1}", product.ID, product.Name);
    }
}
```

###### Point of View 1: Sending object as log

In this point of view, you can send your object as parameter. And send your parameters to cause exception as extra. Sample is below

```csharp
static void SaveProduct(Product product)
{
    try
    {
        ParamDictionary parameters = product.ToParamDictionary();
        JohnsonManager.Data.ExecuteNonQuery("dbo.SaveProduct", parameters);
    }
    catch (Exception ex)
    {
        JohnsonManager.Logger.Log(product, "SaveProduct");
    }
}
```


### 6. JohnsonNet.Operation.MailOperation class

JohnsonNet has a mail operation class

Simply it allows you to send mail from your project.

#### Configuring

##### Step 1: Add config section to your web.config/appconfig

```xml
<appSettings>
    <add key="SmtpUser" value="" />
    <add key="SmtpPass" value="" />
    <add key="SmtpPort" value="" />
    <add key="SmtpServer" value="" />
    <add key="SmtpEnableSSL" value="True" />
</appSettings>
```

##### Step 2: Using

It has cc, bcc, attachments parameters. If it receive an error during sending to mail. It will return to exception for your further use.

```csharp
Exception exception = JohnsonManager.Mail.Send("info@google.com", "Johnson Has Mail Provider", "Your Body");
```

### 7. JohnsonNet.Operation.MultiThreadOperation

You can run asynchronous methods and rename your thread for debugging.

```csharp
JohnsonManager.MultiThread.ExecuteAsync(() =>
{
    for (int i = 0; i < int.MaxValue; i++)
    {

    }
}, name: "BigLoop");
```
