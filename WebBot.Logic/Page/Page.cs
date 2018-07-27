namespace WebBot.Logic.Page
{
    public class Page<ModelType> where ModelType : IPageModel
    {
        public ModelType Model { get; private set; }

        public Page(ModelType model)
        {
            this.Model = model;
        }
    }
}
