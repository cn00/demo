using System;
using UnityEngine;
using System.Collections;

/*--------------------------------------------------
	タッチエフェクトTap
---------------------------------------------------*/
public class AppParticleTap : MonoBehaviour
{
	[SerializeField]private	ParticleSystem	m_particleSakura;
	[SerializeField]private	ParticleSystem	m_particleStar;
	[SerializeField]private	ParticleSystem	m_particleTwinkle;
	[SerializeField]private	ParticleSystem	m_particleCircle;

    [SerializeField]private	int		m_sakuraCount;
	[SerializeField]private	int		m_starCount;
	[SerializeField]private	int		m_twinkleCount;
	[SerializeField]private	int		m_circleCount;
	[SerializeField]private	bool	m_isOn;
	
	[SerializeField]private Camera camera;

	/*--------------------------------------------------
		コンストラクタ
	---------------------------------------------------*/
	void
	Awake()
	{
		m_sakuraCount	= m_particleSakura.maxParticles;
		m_starCount		= m_particleStar.maxParticles;
		m_twinkleCount	= m_particleTwinkle.maxParticles;
		m_circleCount	= m_particleCircle.maxParticles;
	}

	public Vector2 mp;

	public Vector2 wp;

	void OnMouseDown()
	{
		if (Input.touchCount > 0)
		{
			print(Input.touchCount);
		}
	}
	// void Update()
	// {
	// 	if (Input.touchCount > 0)
	// 	{
	// 		var tc = Input.touches[0];
	// 		mp = tc.position;//Input.mousePosition;
	// 		wp = camera.ScreenToWorldPoint(mp);
	// 		On(wp);
	// 	}
	// 	else
	// 		Off();
	// }

	/*--------------------------------------------------
		表示
	---------------------------------------------------*/
	public void
	On( Vector3 pos )
	{
		if( m_isOn == true )
			return;

		gameObject.transform.position	= new Vector3( pos.x, pos.y, 0.0f );
		m_isOn	= true;

		m_particleSakura.Emit( m_sakuraCount );
		m_particleStar.Emit( m_starCount );
		m_particleTwinkle.Emit( m_twinkleCount );
		m_particleCircle.Emit( m_circleCount);
	}

	/*--------------------------------------------------
		非表示
	---------------------------------------------------*/
	public void
	Off()
	{
		if( m_isOn == true )
			m_isOn	= false;
	}
}
