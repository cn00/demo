
### MonoBehaviour
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

### table chars
```
┏--┳--┓
|  |  |
┣--╋--┫
|  |  |
┗--┻--┛
```

### [Getting started with Burst](https://connect.unity.com/p/burst-bian-yi-qi-ru-men-wu)
- Burst 提高性能很容易
- 只修要投入 1% 的时间就能收获 99% 的性能优化
- 写起来也很容易, 复制拷贝一气合成

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