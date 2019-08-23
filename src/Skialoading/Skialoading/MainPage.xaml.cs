using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using SkiaDemo.Mountain;
using Skialoading.Loading;
using SkiaLoading.Combination;
using SkiaLoading.Fireworks;
using SkiaLoading.Smoke;
using SkiaLoading.Sprite;
using SkiaLoading.Stars;
using Xamarin.Forms;

namespace Skialoading
{
    [DesignTimeVisible(false)]
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
            BindingContext = this;
        }

        public ICommand OpenLoadingCommand => new Command(o => Push(new LoadingPage()));

        public ICommand OpenMountainCommand => new Command(o => Push(new MountainPage()));

        public ICommand OpenStarsCommand => new Command(o => Push(new StarPage()));

        public ICommand OpenCombinationCommand => new Command(o => Push(new CombinationPage()));

        public ICommand OpenSpriteAnimationCommand => new Command(o => Push(new SpriteAnimationPage()));

        public ICommand OpenSmokeCommand => new Command(o => Push(new SmokePage()));

        public ICommand OpenFireworksCommand => new Command(o => Push(new FireworksPage()));

        private async void Push(Page page) => await Navigation.PushAsync(page);
    }
}
