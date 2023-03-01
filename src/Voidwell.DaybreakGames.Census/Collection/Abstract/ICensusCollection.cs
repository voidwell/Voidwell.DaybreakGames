namespace Voidwell.DaybreakGames.Census.Collection.Abstract
{
    public interface ICensusCollection<TCensusType> : ICensusCollection where TCensusType : class
    {
    }

    public interface ICensusCollection
    {
        public string CollectionName { get; }
    }
}
