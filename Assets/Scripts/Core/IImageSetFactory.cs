using ScenarioModel;

namespace Core
{
    public interface IImageSetFactory
    {
        void CreateAndAdd(IImageAdder stacker, ImageOrder order);
    }
}