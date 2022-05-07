﻿using System;
using System.Windows.Input;

namespace DownloadManager.ViewModels;

public class Command : ICommand
{
    private readonly Action<object?> _execute;
    private readonly Func<object?, bool>? _canExecute;

    public Command(Action<object?> execute, Func<object?, bool> canExecute = null)
    {
        _execute = execute;
        _canExecute = canExecute;
    }

    public bool CanExecute(object? parameter) => 
        _canExecute?.Invoke(parameter) ?? true;

    public void Execute(object? parameter) => 
        _execute(parameter);

    public event EventHandler? CanExecuteChanged;
}