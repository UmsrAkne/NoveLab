namespace Core
{
    public interface IImageContainer
    {
        void AddImage(IDisplayImage image);

        IDisplayImage GetFront();
    }
}