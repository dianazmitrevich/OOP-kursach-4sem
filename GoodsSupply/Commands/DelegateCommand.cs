using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoodsSupply.Commands
{
    class DelegateCommand : Command
    {
        private readonly Func<object, bool> canExecute;
        private readonly Action<object> execute;

        public DelegateCommand(Action<object> executeParameter, Func<object, bool> canExecuteParameter = null)
        {
            execute = executeParameter ?? throw new ArgumentNullException(nameof(Execute));
            canExecute = canExecuteParameter;
        }

        public override bool CanExecute(object parameter) => canExecute?.Invoke(parameter) ?? true;

        public override void Execute(object parameter) => execute(parameter);
    }
}
