using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Codelisk.Base.ViewModels
{
    public abstract class BaseBaseVm : ReactiveObject
    {
        CompositeDisposable? deactivateWith;
        protected CompositeDisposable DeactivateWith => this.deactivateWith ??= new CompositeDisposable();
        protected CompositeDisposable DestroyWith { get; }


        protected virtual void Deactivate()
        {
            this.deactivateWith?.Dispose();
            this.deactivateWith = null;
        }

        private bool _isBusy;
        public bool IsBusy
        {
            get { return _isBusy; }
            set { this.RaiseAndSetIfChanged(ref _isBusy, value); }
        }
        /*ReactiveStartMark [Reactive1] public string? Title { get; protected set; }*/
        private string? _title;
        public string? Title
        {
            get { return _title; }
            set
            {
                this.RaiseAndSetIfChanged(ref _title, value);
            }
        }//ReactiveEndMark


        protected void BindBusyCommand(ICommand command)
            => this.BindBusyCommand((IReactiveCommand)command);

        protected void BindBusyCommand(IReactiveCommand command) =>
            command.IsExecuting.Subscribe(
                x => this.IsBusy = x,
                _ => this.IsBusy = false,
                () => this.IsBusy = false
            )
            .DisposeWith(this.DeactivateWith);
    }
}
