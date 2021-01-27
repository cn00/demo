using UnityEngine;
using System.Collections;

/*--------------------------------------------------
	タッチエフェクトDrag
---------------------------------------------------*/
public class AppParticleDrag : MonoBehaviour
{
	[SerializeField]private	ParticleSystem	m_particle;
	
	private	Vector3	m_pos;
	private	bool	m_isOn;

	[SerializeField] private Camera camera;

	private void Update()
	{
		if (Input.GetMouseButton(0))
			On(camera.ScreenToWorldPoint(Input.mousePosition));
		else
			Off();
	}
	
	/*--------------------------------------------------
		表示
	---------------------------------------------------*/
	public void
	On( Vector3 pos )
	{
		//移動したら演出
		if( m_pos.x != pos.x || m_pos.y != pos.y )
		{
			if( m_particle.isStopped )
			{
				m_isOn	= true;
				m_particle.Play();
			}

			gameObject.transform.position = new Vector3(pos.x, pos.y, 0.0f);
		}

		m_pos	= pos;
	}

	/*--------------------------------------------------
		非表示
	---------------------------------------------------*/
	public void
	Off()
	{
		if( m_isOn == true )
		{
			m_isOn	= false;
			m_particle.Stop();
		}
	}
}
