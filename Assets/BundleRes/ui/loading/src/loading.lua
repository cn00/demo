
local loading = {
	
}
local self = loading

--AutoGenInit Begin
function loading.AutoGenInit()
    loading.Cube = Cube:GetComponent("UnityEngine.MeshRenderer")
end
--AutoGenInit End

function loading.Start ()
    print("loading.Start")

end

function loading:FixedUpdate ()

end

function loading.Update ()

end

function loading:LateUpdate ()

end

function loading.OnDestroy ()
    print("loading.OnDestroy")

end
    
return loading
