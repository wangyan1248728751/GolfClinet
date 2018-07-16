using GameFramework.DataTable;
using System.Collections.Generic;

namespace Golf
{
    /// <summary>
    /// 岛屿奖励表。
    /// </summary>
    public class DRRewardMap : IDataRow
    {
        /// <summary>
        /// 岛屿编号。
        /// </summary>
        public int Id { get; set; }

		/// <summary>
		/// 地块得分。
		/// </summary>
		public int Score { get; set; }

		public string ScoreColor { get; set; }

		public int goodsId { get; set; }

        /// <summary>
        /// 奖励物品图片链接地址。
        /// </summary>
        public string ImageUrl { get; set; }


		public void ParseDataRow(string dataRowText)
        {
            string[] text = DataTableExtension.SplitDataRow(dataRowText);
            int index = 0;
            index++;
            Id = int.Parse(text[index++]);
            index++;
            Score = int.Parse(text[index++]);
			ScoreColor = text[index++];
            ImageUrl = text[index++];
        }

        private void AvoidJIT()
        {
            new Dictionary<int, DRRewardMap>();
        }
    }
}
