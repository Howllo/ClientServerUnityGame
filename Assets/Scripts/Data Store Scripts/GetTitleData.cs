namespace TitleDateInfo
{
    public class GetTitleData
    {
        /// <summary>
        /// The minimum required streak to get a reward.
        /// </summary>
        public long MinStreak { get; set; }
        /// <summary>
        /// Reward that is given to the player.
        /// </summary>
        public string Reward { get; set; }
        /// <summary>
        /// Use to get the catalog that the item is in.
        /// </summary>
        public string Catalog { get; set; }
    }
}