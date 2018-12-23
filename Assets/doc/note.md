
### MonoBehaviour
```
Reset                                           
Awake                                           
OnEnable <--------------------------------┓     
Start                                     |     
FixedUpdate <--------------------┓        |     
yield WaitForFixedUpdate         |        |     
[Internal physics update]        |        |     
OnTriggerXXX                     |        |     
OnCollisionXXX ----------------->┤        |     
OnMouseXXX                       |        |     
Update                           |        |     
yield null                       |        |     
yield WaitForSeconds             |        |     
yield WWW                        |        |     
yield StartCoroutine             |        |     
[internal animation update]      |        |     
LaterUpdate                      |        |     
OnWillRenderObject               |        |     
OnPreCull                        |        |     
OnBecameVisible                  |        |     
OnBecameInvisible                |        |     
OnPrerender                      |        |     
OnRenderObject                   |        |     
OnPostRender                     |        |     
OnRenderImage                    |        |     
OndrawGizmos                     |        |     
OnGUI ------><------             |        |     
yield WaitForEndOfFrame          |        |     
OnApplicationPause ------------->┛        |     
OnDisable --------------------------------┛     
OnDestroy                                       
OnApplicationQuit                               
```

### table chars
```
┏--┳--┓
|  |  |
┣--╋--┫
|  |  |
┗--┻--┛
```


### idfa
```objc
- (NSString *)identifierForAdvertising {
    // Check whether advertising tracking is enabled
    if([[ASIdentifierManager sharedManager] isAdvertisingTrackingEnabled]) {
        NSUUID *identifier = [[ASIdentifierManager sharedManager] advertisingIdentifier];
        return [identifier UUIDString];
    }

    // Get and return IDFA
    return nil;
}
```