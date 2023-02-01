namespace tDarkBot.Models
{
    public class ProfileModel
    {
        public string Name { get; set; }
        public string? AvatarURL { get; set; }
        public string? BannerURL { get; set; }

        public ProfileModel(string name)
        {
            Name = name;
        }
    }
}