Bookstore is a simple web-based application which can display basic information about books finds by bookstore providers.

To use Amazon provider you have to put your keys to appSettings section in Web.config

Example:

```xml
<appSettings>
  <add key="AmazonApiKey" value="YOUR_API_KEY_HERE"/>
  <add key="AmazonSecret" value="YOUR_SECRET_HERE"/>
  <add key="AmazonAssociateTag" value="YOUR_ASSOCIATETAG_HERE"/>
<appSettings>
```