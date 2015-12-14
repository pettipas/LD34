using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class MaterialFlasher : MonoBehaviour {
	
	public bool animateOnAwake;
	
	public Color flashA;
	public Color flashB;
	public Color flashC;
	public Color[] m_Colors;
	Color currentColor;
	int index = 0;
	int nextIndex;
	float speed = 0.07f;
	float startTime = 0f;
	float progress = 0f;
	public bool ignoreDiration;
	public List<MeshRenderer> renderers = new List<MeshRenderer>();
	List<Color> colors = new List<Color>();
	[SerializeField]
	protected bool flash;
	public bool Flash{
		get{
			return flash;
		}
		set{
			
			flash = value;
			if(value == false){
				for(int i = 0;i< renderers.Count;i++){
					renderers[i].material.color = colors[i];
				}
			}
		}
	}
	
	void Awake(){
		Init();
	}
	
	public void Init(){
		m_Colors = new Color[3];
		m_Colors[0] = flashA;
		m_Colors[1] = flashB;
		m_Colors[2] = flashC;
		
		if(renderers.Count <= 0){
			renderers = GetComponentsInChildren<MeshRenderer>().ToList();
		}
		
		renderers.ForEach(r=>{
			colors.Add(r.material.color);
		});
		
		
		if(m_Colors.Length > 0){
			currentColor = m_Colors[index];
			nextIndex = (index+1)%m_Colors.Length;
		}
		
		if(animateOnAwake){
			Flash = true;
			timer = 0;
		}
	}
	
	float timer = 0;
	float cooldown = 0;
	public void FlashForTime(float duration){
		Flash = true;
		timer = 0;
		cooldown = duration;
	}
	
	void Update () {
		if(!Flash){
			return;
		}
		timer+=Time.deltaTime;
		progress = (Time.time-startTime)/speed;
		if(progress >= 1){
			nextIndex = (index+2)%m_Colors.Length;
			index = (index+1)%m_Colors.Length;
			startTime = Time.time;
		}else{
			currentColor = Color.Lerp(m_Colors[index], m_Colors[nextIndex], progress);
		}
		
		for(int i = 0;i< renderers.Count;i++){
			renderers[i].material.color = currentColor;
		}
		
		if(timer > cooldown && !animateOnAwake && !ignoreDiration){
			Flash = false;
		}
	}
}
