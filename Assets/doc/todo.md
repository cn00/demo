
## MonoBehaviour
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
┏--┳--┓
|  |  |
┣--╋--┫
|  |  |
┗--┻--┛
```

### update
* [x] bundle pack
* [ ] version controll


### qr
* Android
* iOS