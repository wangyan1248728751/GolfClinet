using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;
using UnityEngine.UI;

/**
 * Apply on GUIText GameObject
 * */
public class UnityWebView : MonoBehaviour
{
	public int width, height;
    private GUITexture gui;
	private Texture2D m_texture;    
	public int m_TextureID;
    private Color[] m_pixels = null;
    private GCHandle m_pixelsHandler;    
	public bool m_bForceOpenGL = true;
    public bool m_bInitialized = false;

    //private ControlWindow controlWindow;
    private UnityWebViewEvents browserEventHandler;
	
	private UnityWebCore.BeginNavigationDelFunc 		beginNavigationDelFunc;
	private UnityWebCore.BeginLoadingDelFunc			beginLoadingDelFunc;
	private UnityWebCore.FinishLoadingDelFunc 		finishLoadingDelFunc;
	private UnityWebCore.ReceiveTitleDelFunc 			receiveTitleDelFunc;
	private UnityWebCore.ChangeTooltipDelFunc 		changeTooltipDelFunc;
	private UnityWebCore.ChangeTargetURLDelFunc 	changeTargetURLDelFunc;
	private UnityWebCore.ChangeCursorDelFunc 		changeCursorDelFunc;
	private UnityWebCore.JSCallbackDelFunc 			showControlWindowFunc;	

    // Use this for initialization
    void Start()
    {
        browserEventHandler = GetComponent<UnityWebViewEvents>();
        //controlWindow = GameObject.Find("ControlWindow").GetComponent<ControlWindow>();
        Init(600, 600);      
    }
	
	void OnApplicationQuit()
    {
         UnityWebCore.DestroyView(m_TextureID);
    }

    public void Init(int width, int height)
    {
        this.width = width;
        this.height = height;
        m_texture = new Texture2D(width, height, TextureFormat.ARGB32, false);

		
		m_TextureID = m_texture.GetNativeTextureID();
		
		if(m_bForceOpenGL && !Application.isEditor)
		{
			UnityWebCore.CreateView(m_TextureID, new System.IntPtr(0), width, height, false, 10);
		}	
		else
		{
			 //Get Color[] (pixels) from texture 
			m_pixels = m_texture.GetPixels(0);
			// Create GCHandle - Allocation of m_pixels in memory. 
			m_pixelsHandler = GCHandle.Alloc(m_pixels, GCHandleType.Pinned);       
			
			UnityWebCore.CreateView(m_TextureID, m_pixelsHandler.AddrOfPinnedObject(), width, height, true, 10);
		}

		// assign m_texture to this GUITexture texture        
		//gameObject.GetComponent<Renderer>().material.mainTexture = m_texture;
		//Sprite sprites = Sprite.Create(m_texture, new Rect(0, 0, m_texture.width, m_texture.height), new Vector2(0.5f, 0.5f));
		//QRImage.sprite = sprites;
		gameObject.GetComponent<RawImage>().texture = m_texture;

		//beginNavigationDelFunc = new UnityWebCore.BeginNavigationDelFunc(this.onBeginNavigationDelFunc);
		//beginLoadingDelFunc = new UnityWebCore.BeginLoadingDelFunc(this.onBeginLoadingDelFunc);
		//finishLoadingDelFunc = new UnityWebCore.FinishLoadingDelFunc(this.onFinishLoadingDelFunc);
		//receiveTitleDelFunc = new UnityWebCore.ReceiveTitleDelFunc(this.onReceiveTitleDelFunc);
		//changeTooltipDelFunc = new UnityWebCore.ChangeTooltipDelFunc(this.onChangeTooltipDelFunc);
		//changeTargetURLDelFunc = new UnityWebCore.ChangeTargetURLDelFunc(this.onChangeTargetURLDelFunc);
		//changeCursorDelFunc = new UnityWebCore.ChangeCursorDelFunc(this.onChangeCursorDelFunc);
		//showControlWindowFunc = new UnityWebCore.JSCallbackDelFunc(this.onShowControlWindow);

		//UnityWebCore.SetBeginNavigationFunc(m_TextureID, beginNavigationDelFunc);	
		//UnityWebCore.SetBeginLoadingFunc(m_TextureID, beginLoadingDelFunc);	
		//UnityWebCore.SetFinishLoadingFunc(m_TextureID, finishLoadingDelFunc);	
		//UnityWebCore.SetReceiveTitleFunc(m_TextureID, receiveTitleDelFunc);	
		//UnityWebCore.SetChangeTooltipFunc(m_TextureID, changeTooltipDelFunc);	
		//UnityWebCore.SetChangeTargetURLFunc(m_TextureID, changeTargetURLDelFunc);	
		//UnityWebCore.SetChangeCursorFunc(m_TextureID, changeCursorDelFunc);	
		//UnityWebCore.AddJSCallback(m_TextureID, "showControlWindow", showControlWindowFunc);

		browserEventHandler.setDimensions(width, height);

        m_bInitialized = true;

        browserEventHandler.interactive = true;
    }

    // Update is called once per frame
    void Update()
    {
        if ( m_bInitialized/* && controlWindow.showBrowser*/)
        {           
            UnityWebCore.Update();           
            if (UnityWebCore.IsDirty(m_TextureID))
            {                
                m_texture.SetPixels(m_pixels, 0);
                m_texture.Apply();
            }
        }

        if (browserEventHandler.focused && Input.inputString.Length > 0)
        {
            // Send WM_CHAR message
            for(int i = 0; i < Input.inputString.Length; ++i)
            {
                // Backspace - Remove the last character
                if (Input.inputString[i] == '\b') 
                {
					//Debug.Log("backspace");
					UnityWebCore.InjectKeyboard(m_TextureID, 0x0100, 0x08, 0);
					UnityWebCore.InjectKeyboard(m_TextureID, 0x0101, 0x08, 0);
				}
				else if(Input.inputString[i] == '\r' || Input.inputString[i] == '\n')
				{
					//Debug.Log("enter");
					UnityWebCore.InjectKeyboard(m_TextureID, 0x0100, 0x0D, 0);
					UnityWebCore.InjectKeyboard(m_TextureID, 0x0101, 0x0D, 0);
				}
                else
                    UnityWebCore.InjectKeyboard(m_TextureID, 0x0102, Input.inputString[i], 0);
            }
        }
		
		int dyScroll = (int)Input.GetAxis("Mouse ScrollWheel");
        if (dyScroll != 0)
        {
			//Debug.Log("Mouse Scroll" + dyScroll);
            UnityWebCore.MouseWheel(m_TextureID, dyScroll);
        }
		
		if(browserEventHandler.hovered)
		{
			if(Input.GetMouseButtonDown (2))
				browserEventHandler.handleMouseDown(1);
			else if(Input.GetMouseButtonDown (1))
				browserEventHandler.handleMouseDown(2);
			else if(Input.GetMouseButtonUp (2))
				browserEventHandler.handleMouseUp(1);
			else if(Input.GetMouseButtonUp (1))
				browserEventHandler.handleMouseUp(2);
		}

    }

    public UnityWebViewEvents getEventHandler()
    {
        return browserEventHandler;
    }
	
	//public void setFPS(float fps)
	//{
	//	object[] args = new object[1];
	//	args[0] = fps;
	//	UnityWebCore.CallJS(m_TextureID, "setFPS", args);
	//	//Debug.Log(fps);
	//}

	//public void onShowControlWindow(string arguments)
	//{
	//	object[] args = UnityWebCore.StringToArgs(arguments);
	//	controlWindow.bVisible = (bool)args[0];			
	//}
	//public void onBeginNavigationDelFunc(string frameName, string url)
	//{
	//	controlWindow.statusText = "Begin navigating to [" + frameName + "]"  + url;
	//	//Debug.Log("BeginNavigation to " + url);
	//}
	
 //   public void onBeginLoadingDelFunc(string frameName, string url, int statusCode, string mimeType)
	//{
	//	controlWindow.statusText = "Begin loading [" + frameName + "]"  + url;
	//	//Debug.Log("BeginLoading: " + url);
	//}
 //   public void onFinishLoadingDelFunc()
	//{
	//	//Debug.Log("FinishLoading");
	//	controlWindow.statusText = "Finish loading" ;
	//}
	
	//public void onReceiveTitleDelFunc(string frameName, string title)
	//{
	//	//Debug.Log("ReceiveTitle: " + title);
	//	controlWindow.statusText = "Receive title [" + frameName + "]" + title;
	//}
	
 //   public void onChangeTooltipDelFunc(string tooltip)
	//{
	//	controlWindow.statusText = "Change tootip " + tooltip;
	//}
	
	//public void onChangeCursorDelFunc(int cursorID)
	//{
	//	UnityWebCore.ApplyCursor(cursorID);
		
	//	controlWindow.statusText = "Change cursor " + cursorID;
	//}
	
    public void onChangeTargetURLDelFunc(string url)
	{
		//Debug.Log("ChangeTargetURL: " + url);
	}
	
    public void Loadfile(string filePath)
    {
        if (m_bInitialized)
            UnityWebCore.LoadFile(m_TextureID,filePath);
    }

    public void LoadURL(string url)
    {
        if (m_bInitialized)
            UnityWebCore.LoadURL(m_TextureID, url);
    }

    public void Destroy()
    {
        try
        {
            if (m_TextureID != 0)
            {
                UnityWebCore.DestroyView(m_TextureID);
				if(m_pixels != null)
					m_pixelsHandler.Free();
				Destroy(m_texture);
                GetComponent<UnityWebViewEvents>().interactive = false;
                m_TextureID = 0;

                m_bInitialized = false;
            }
        }
        catch (System.Exception e)
        {
            Debug.Log(e);
        }       
    }
}
