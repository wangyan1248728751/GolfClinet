using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Golf;

public class RankListItem : MonoBehaviour {

	[SerializeField]
	Text m_id;
	[SerializeField]
	Text m_name;
	[SerializeField]
	Text m_score;

	public void SetData(RankData rankData)
	{
		//TODO:Rank图标

		m_id.text = (rankData.rank + 1).ToString();
		m_name.text = rankData.name;
		m_score.text = rankData.score.ToString();

	}


}
