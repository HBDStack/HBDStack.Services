# Introduction 
The extension services for Mef.

### 1. HBD.Services.Configuration.
The service allows loading the configuration from various sources. The configuration will cache automatically to speed up the performance and avoid to reload the config from source frequently.
However, when caching is expired, or the is a new version of config ready. The configuration will be re-load automatically.

### 2. HBD.Services.Caching

Providing the caching for .Net application available for all .Net Framework from 4.5.2 to Standard 2.0

Currently, only MemoryCache is available.

### 3. HBD.Services.Singleton

This project had been separated from HBD.Framework, providing the SingletonManager for .Net application. 

