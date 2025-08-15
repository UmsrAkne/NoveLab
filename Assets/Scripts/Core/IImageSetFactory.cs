using ScenarioModel;

namespace Core
{
    public interface IImageSetFactory
    {
        void CreateAndAdd(IImageContainer stacker, ImageOrder order);
    }
}