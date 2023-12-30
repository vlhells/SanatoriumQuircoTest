## Namespace ("учётная запись компании"):  
Previously, application services could register with a homeserver via HTTP APIs.  
This was removed as it was seen as a security risk.  
A compromised application service could re-register for a global  
* regex and sniff all traffic on the homeserver.  
To protect against this, application services now have to  
register via configuration files which are linked to the homeserver  
configuration file. The addition of configuration files allows homeserver admins  
to sanity check the registration for suspicious regex strings.    
https://spec.matrix.org/v1.9/application-service-api/    

## Аватары пользователям через API:  
В spec не нашёл.    
    
Остальное написал, тестирую.