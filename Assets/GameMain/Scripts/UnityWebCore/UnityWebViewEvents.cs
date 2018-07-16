using UnityEngine;
using System.Collections;

public class UnityWebViewEvents : MonoBehaviour
{
    public bool hovered = false;
    public bool focused = false;
    public bool interactive = true;
    public UnityWebView view;
    private int width, height;
	private int lastX, lastY;

    void Start()
    {
        width = view.width;
        height = view.height;      
    }

    public void setDimensions(int width, int height)
    {
        this.width = width;
        this.height = height;
    }
	
	public void handleMouseDown(int id)
	{
		//Debug.Log("MouseDown" );

        focused = true;

        RaycastHit hit;
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit))
        {           

            int x = /*width -*/ (int)(hit.textureCoord.x * width);
            int y = height - (int)(hit.textureCoord.y * height);

            UnityWebCore.MouseDown(view.m_TextureID, id, x, y);  
			lastX = x;
			lastY = y;
        }
	}
	
    void OnMouseDown()
    {
        // Only when interactive is enabled
        if (!interactive)
            return;
		
		handleMouseDown(0);
    }
	
	public void handleMouseUp(int id)
	{
		//Debug.Log("MouseUp" );
        RaycastHit hit;
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit))
        {            
            int x = /*width -*/ (int)(hit.textureCoord.x * width);
            int y = height - (int)(hit.textureCoord.y * height);
            UnityWebCore.MouseUp(view.m_TextureID, id, x, y);
        }
	}

    void OnMouseUp()
    {
        // Only when interactive is enabled
        if (!interactive)
            return;
        
		handleMouseUp(0);
    }
    

    void OnMouseOver()
    {		
        // Only when interactive is enabled
        if (!interactive)
            return;

        hovered = true;
      
        RaycastHit hit;
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit))
        {         
            int x = /*width -*/ (int)(hit.textureCoord.x * width);
            int y = height - (int)(hit.textureCoord.y * height);
            UnityWebCore.MouseMove(view.m_TextureID, x, y); 
			lastX = x;
			lastY = y;		

			//UnityWebCore.ApplyCursor(1);
        }
    }


    void OnMouseExit()
    {
        // Only when interactive is enabled
        if (!interactive)
            return;

        hovered = false;
        focused = false;
		
		//Debug.Log("MouseOut");
		UnityWebCore.ApplyCursor(0);

		UnityWebCore.MouseUp(view.m_TextureID, 0, lastX, lastY);
		UnityWebCore.MouseUp(view.m_TextureID, 1, lastX, lastY);
		UnityWebCore.MouseUp(view.m_TextureID, 2, lastX, lastY);
        UnityWebCore.MouseMove(view.m_TextureID, -1, -1);
    }
}
