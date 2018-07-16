using Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using UnityEngine;

public static class WatchmeFile
{
	private const string FileName = "ShotHistory.xml";

	private static void AddShotDataNode(XmlDocument doc, XmlNode rootNode, string newNodeName, string newNodeData)
	{
		XmlElement xmlElement = doc.CreateElement(newNodeName);
		xmlElement.InnerText = newNodeData;
		rootNode.AppendChild(xmlElement);
	}

	public static void SaveShot(TFlightData flightdata, Shot shotdata)
	{
		try
		{
			string str = Path.Combine(Application.persistentDataPath, "ShotHistory.xml");
			XmlDocument xmlDocument = new XmlDocument();
			if (!File.Exists(str))
			{
				xmlDocument.AppendChild(xmlDocument.CreateElement("XML"));
			}
			else
			{
				using (FileStream fileStream = File.OpenRead(str))
				{
					xmlDocument.Load(fileStream);
				}
			}
			XmlNode xmlNodes = xmlDocument.SelectSingleNode("XML");
			XmlElement xmlElement = xmlDocument.CreateElement("SHOT");
			WatchmeFile.AddShotDataNode(xmlDocument, xmlElement, "DATETIME", flightdata.dateOfHit.ToString("G"));
			WatchmeFile.AddShotDataNode(xmlDocument, xmlElement, "ESN", flightdata.ESN);
			int id = LoginManager.UserData.Id;
			WatchmeFile.AddShotDataNode(xmlDocument, xmlElement, "CUSTOMERID", id.ToString());
			WatchmeFile.AddShotDataNode(xmlDocument, xmlElement, "CLUBNAME", flightdata.clubName);
			WatchmeFile.AddShotDataNode(xmlDocument, xmlElement, "CLUBTYPE", flightdata.clubTypeID);
			WatchmeFile.AddShotDataNode(xmlDocument, xmlElement, "SHOTID", flightdata.esnShotId.ToString());
			int numberInActivity = shotdata.NumberInActivity;
			WatchmeFile.AddShotDataNode(xmlDocument, xmlElement, "SHOTNUMBER", numberInActivity.ToString());
			WatchmeFile.AddShotDataNode(xmlDocument, xmlElement, "UOM", "I");
			WatchmeFile.AddShotDataNode(xmlDocument, xmlElement, "ALTITUDE", WeatherManager.instance.altitude.ToString());
			WatchmeFile.AddShotDataNode(xmlDocument, xmlElement, "BALLSPEED", flightdata.totalSpeedMPH.ToString());
			WatchmeFile.AddShotDataNode(xmlDocument, xmlElement, "CLUBHEADSPEED", flightdata.clubSpeed.ToString());
			WatchmeFile.AddShotDataNode(xmlDocument, xmlElement, "SMASHFACTOR", flightdata.smashFactor.ToString());
			WatchmeFile.AddShotDataNode(xmlDocument, xmlElement, "VERTICALLAUNCHANGLE", flightdata.launchAngle.ToString());
			WatchmeFile.AddShotDataNode(xmlDocument, xmlElement, "HORIZONTALLAUNCHANGLE", flightdata.horizontalAngle.ToString());
			WatchmeFile.AddShotDataNode(xmlDocument, xmlElement, "BACKSPIN", flightdata.backSpin.ToString());
			WatchmeFile.AddShotDataNode(xmlDocument, xmlElement, "SIDESPIN", flightdata.sideSpin.ToString());
			WatchmeFile.AddShotDataNode(xmlDocument, xmlElement, "TOTALSPIN", flightdata.totalSpin.ToString());
			WatchmeFile.AddShotDataNode(xmlDocument, xmlElement, "SPINAXIS", flightdata.spinAxis.ToString());
			WatchmeFile.AddShotDataNode(xmlDocument, xmlElement, "CARRYDISTANCE", flightdata.carry.ToString());
			WatchmeFile.AddShotDataNode(xmlDocument, xmlElement, "ROLLDISTANCE", flightdata.roll.ToString());
			float single = flightdata.carry + flightdata.roll;
			WatchmeFile.AddShotDataNode(xmlDocument, xmlElement, "TOTALDISTANCE", single.ToString());
			WatchmeFile.AddShotDataNode(xmlDocument, xmlElement, "OFFLINE", flightdata.offline.ToString());
			WatchmeFile.AddShotDataNode(xmlDocument, xmlElement, "MAXHEIGHT", flightdata.maxHeight.ToString());
			WatchmeFile.AddShotDataNode(xmlDocument, xmlElement, "FLIGHTTIME", (flightdata.trajectory == null ? string.Empty : flightdata.trajectory[flightdata.trajectory.Count - 1].time.ToString()));
			float descentAngle = CBallFlightManager.GetInstance().DescentAngle;
			WatchmeFile.AddShotDataNode(xmlDocument, xmlElement, "DESCENTANGLE", descentAngle.ToString("F"));
			xmlNodes.AppendChild(xmlElement);
			using (FileStream fileStream1 = File.OpenWrite(str))
			{
				xmlDocument.Save(fileStream1);
			}
			FileInfo fileInfo = new FileInfo(str);
			AppLog.Log(string.Concat("Watch-me file saved at path: ", fileInfo.FullName), true);
		}
		catch (Exception exception1)
		{
			Exception exception = exception1;
			AppLog.Log(string.Format("Exception during open/write watch-me file {0}. Message: {1}", "ShotHistory.xml", exception.Message), true);
		}
	}
}