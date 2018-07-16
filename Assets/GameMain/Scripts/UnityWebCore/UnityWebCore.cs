using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;
using System;

public class UnityWebCore
{
    public delegate void BeginNavigationDelFunc(string frameName, string url);
    public delegate void BeginLoadingDelFunc(string frameName, string url, int statusCode, string mimeType);
    public delegate void FinishLoadingDelFunc();
    public delegate void ReceiveTitleDelFunc(string frameName, string title);
    public delegate void ChangeTooltipDelFunc(string tooltip);
    public delegate void ChangeTargetURLDelFunc(string url);	
	
	// cursorID:
	// 0--ARROW
	// 1--HAND
	// 2--IBEAM
	// 3--WAIT
	// 4--SIZE
	// 5--SIZEALL
	// 6--SIZENESW
	// 7--SIZENS
	// 8-SIZENWSE
	// 9--SIZEWE
	// 10--UPARROW
	public delegate void ChangeCursorDelFunc(int cursorID);
	public delegate void JSCallbackDelFunc(string arguments);	
	
	[DllImport("UnityWinUtility")]
    public static extern void EnableCustomCursor(bool bEnable);
	
	[DllImport("UnityWinUtility")]
    public static extern void RegistCursor(int cursorID, string filename);

	[DllImport("UnityWinUtility")]
    public static extern void ApplyCursor(int cursorID);
	
    [DllImport("UnityWebCore")]
    public static extern bool CreateView(int uniqueID, IntPtr pixelBuffer, int width, int height, bool transparent, int maxFPS);

    [DllImport("UnityWebCore")]
    public static extern bool DestroyView(int uniqueID);

    [DllImport("UnityWebCore")]
    public static extern void LoadURL(int uniqueId, string url);

    [DllImport("UnityWebCore")]
    public static extern void LoadFile(int uniqueId, string url);  
    
    [DllImport("UnityWebCore")]
    public static extern void Update();    

    [DllImport("UnityWebCore")]
    public static extern bool IsDirty(int uniqueId);

    [DllImport("UnityWebCore")]
    public static extern void EnableView(int uniqueId, bool bEnable);   

    //Mouse functions
    [DllImport("UnityWebCore")]
    public static extern void MouseMove(int uniqueId, int x, int y);

    // 0 = left mouse, 1 = middle mouse, 2 = right mouse
    [DllImport("UnityWebCore")]
    public static extern void MouseDown(int uniqueId, int mouseBtn, int x, int y);

    [DllImport("UnityWebCore")]
    public static extern void MouseUp(int uniqueId, int mouseBtn, int x, int y);

    [DllImport("UnityWebCore")]
    public static extern void MouseWheel(int uniqueId, int amount);
   
    // Key func
    [DllImport("UnityWebCore")]
    public static extern void InjectKeyboard(int uniqueId, int msg, uint wParam, long lParam);    

    [DllImport("UnityWebCore")]
    public static extern void SetBeginNavigationFunc(int uniqueId, BeginNavigationDelFunc func);    

	[DllImport("UnityWebCore")]
    public static extern void SetBeginLoadingFunc(int uniqueId, BeginLoadingDelFunc func);    

	[DllImport("UnityWebCore")]
    public static extern void SetFinishLoadingFunc(int uniqueId, FinishLoadingDelFunc func);    

	[DllImport("UnityWebCore")]
    public static extern void SetReceiveTitleFunc(int uniqueId, ReceiveTitleDelFunc func);    

	[DllImport("UnityWebCore")]
    public static extern void SetChangeTooltipFunc(int uniqueId, ChangeTooltipDelFunc func);    

	[DllImport("UnityWebCore")]
    public static extern void SetChangeTargetURLFunc(int uniqueId, ChangeTargetURLDelFunc func);    

	[DllImport("UnityWebCore")]
    public static extern void SetChangeCursorFunc(int uniqueId, ChangeCursorDelFunc func);    

	[DllImport("UnityWebCore")]
    public static extern void ExecuteJS(int uniqueId, string javascript);    
	
	[DllImport("UnityWebCore")]
	public static extern void AddJSCallback(int uniqueId, string funcName, JSCallbackDelFunc func);
	
	[DllImport("UnityWebCore")]
	public static extern void RemoveJSCallback(int uniqueId, string funcName);
	
	[DllImport("UnityWebCore")]
    private static extern void CallJS(int uniqueId, string funcName, string arguments, string frameName);
    public static string ArgsToString(params  object[] arguments)
    {
        string result = "";

        if (arguments.Length > 0)
            result += arguments.Length.ToString();

        foreach (object obj in arguments)
        {
            if (obj is int)
            {
                int i = (int)obj;
                string iStr = i.ToString();
                result += "I" + iStr.Length.ToString() + ";" + iStr;
            }
            else if (obj is float )
            {
                float i = (float)obj;
                string iStr = i.ToString();
                result += "D" + iStr.Length.ToString() + ";" + iStr;
            }
            else if (obj is double)
            {
                double i = (double)obj;
                string iStr = i.ToString();
                result += "D" + iStr.Length.ToString() + ";" + iStr;
            }
            else if (obj is bool)
            {
                bool b = (bool)obj;
                if(b)
                    result += "B1";
                else
                    result += "B0";
            }
            else if (obj is string)
            {
                string iStr = (string)obj;
                result += "S" + iStr.Length.ToString() + ";" + iStr;
            }
            else
                return "";
        }
        return result;
    }
	public static void CallJS(int uniqueId, string funcName, params  object[] arguments)
	{
        CallJS(uniqueId, funcName, ArgsToString(arguments), "");
	}	
	public static object[] StringToArgs(string arguments)
	{
        int length = arguments.Length;
        if(length == 0)
			return new object[0];

        object[] result = new object[int.Parse(arguments.Substring(0, 1))];

		int index = 1;
        int objIndex = 0;

		while (index < length)
		{
			switch (arguments[index])
			{
			case 'B':
				{
					if(arguments[++index] == '1')
						result[objIndex ++] = true;
					else
						result[objIndex ++] = false;

					index++;
				}
				break;
			case 'D':
				{					
					int offset = arguments.IndexOf(';', ++index);
                    Debug.Log(index + " " + offset);
		            int len = int.Parse(arguments.Substring(index, offset - index));
                    index = offset + 1;
                    result[objIndex ++] = float.Parse( arguments.Substring(index, len));                
					index += len;
				}
				break;
			case 'I':
				{
                    int offset = arguments.IndexOf(';', ++index);	
		            int len = int.Parse(arguments.Substring(index, offset - index));
                    index = offset + 1;
                    result[objIndex ++] = int.Parse( arguments.Substring(index, len));                
					index += len;
				}
				break;
			case 'S':
				{
                    int offset = arguments.IndexOf(';', ++index);
                    int len = int.Parse(arguments.Substring(index, offset - index));
                    index = offset + 1;
                    result[objIndex++] = arguments.Substring(index, len);
                    index += len;
				}
				break;
			default:
                return new object[0];
			}
		}

        return result;
	}
}
