namespace WebBot.Logic.Page
{
    public class Page<ModelType> where ModelType : IPageModel
    {
        public ModelType Model { get; private set; }
        public string Text { get; private set; }

        public Page(string text)
        {
            this.Text = text;

            // todo : 모델 만들기
        }
    }
}
