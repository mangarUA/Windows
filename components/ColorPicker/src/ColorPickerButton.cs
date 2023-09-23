// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Windows.UI;

namespace CommunityToolkit.WinUI.Controls;

/// <summary>
/// A <see cref="DropDownButton"/> which displays a color as its <c>Content</c> and it's <c>Flyout</c> is a <see cref="ColorPicker"/>.
/// </summary>
[TemplatePart(Name = nameof(CheckeredBackgroundBorder), Type = typeof(Border))]
public partial class ColorPickerButton : DropDownButton
{
    /// <summary>
    /// Gets the <see cref="Controls.ColorPicker"/> instances contained by the <see cref="DropDownButton"/>.
    /// </summary>
    public ColorPicker ColorPicker { get; private set; }

    /// <summary>
    /// Gets or sets the <see cref="Style"/> for the <see cref="Controls.ColorPicker"/> control used in the button.
    /// </summary>
    public Style ColorPickerStyle
    {
        get
        {
            return (Style)GetValue(ColorPickerStyleProperty);
        }

        set
        {
            SetValue(ColorPickerStyleProperty, value);
        }
    }

    /// <summary>
    /// Identifies the <see cref="ColorPickerStyle"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty ColorPickerStyleProperty = DependencyProperty.Register(nameof(ColorPickerStyle), typeof(Style), typeof(ColorPickerButton), new PropertyMetadata(default(Style)));

    /// <summary>
    /// Gets or sets the <see cref="Style"/> for the <see cref="FlyoutPresenter"/> used within the <see cref="Flyout"/> of the <see cref="DropDownButton"/>.
    /// </summary>
    public Style FlyoutPresenterStyle
    {
        get
        {
            return (Style)GetValue(FlyoutPresenterStyleProperty);
        }

        set
        {
            SetValue(FlyoutPresenterStyleProperty, value);
        }
    }

    /// <summary>
    /// Identifies the <see cref="FlyoutPresenterStyle"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty FlyoutPresenterStyleProperty = DependencyProperty.Register(nameof(FlyoutPresenterStyle), typeof(Style), typeof(ColorPickerButton), new PropertyMetadata(default(Style)));

#pragma warning disable CS0419 // Ambiguous reference in cref attribute
    /// <summary>
    /// Gets or sets the selected <see cref="Windows.UI.Color"/> the user has picked from the <see cref="ColorPicker"/>.
    /// </summary>
#pragma warning restore CS0419 // Ambiguous reference in cref attribute
    public Color SelectedColor
    {
        get { return (Color)GetValue(SelectedColorProperty); }
        set { SetValue(SelectedColorProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="SelectedColor"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty SelectedColorProperty =
        DependencyProperty.Register(nameof(SelectedColor), typeof(Color), typeof(ColorPickerButton), new PropertyMetadata(null, new PropertyChangedCallback(SelectedColorChanged)));

#pragma warning disable SA1306 // Field names should begin with lower-case letter
    //// Template Parts
    private Border? CheckeredBackgroundBorder;
#pragma warning restore SA1306 // Field names should begin with lower-case letter

    /// <summary>
    /// Initializes a new instance of the <see cref="ColorPickerButton"/> class.
    /// </summary>
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    public ColorPickerButton()
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    {
        this.DefaultStyleKey = typeof(ColorPickerButton);

        // Workaround for https://github.com/microsoft/microsoft-ui-xaml/issues/3502
        this.DefaultStyleResourceUri = new Uri("ms-appx:///CommunityToolkit.WinUI.Controls.ColorPicker/Themes/Generic.xaml");
    }

    /// <inheritdoc/>
    protected override void OnApplyTemplate()
    {
        if (ColorPicker != null)
        {
            ColorPicker.ColorChanged -= ColorPicker_ColorChanged;
        }

        base.OnApplyTemplate();

        if (ColorPickerStyle != null)
        {
            ColorPicker = new ColorPicker() { Style = ColorPickerStyle };
        }
        else
        {
            ColorPicker = new ColorPicker();
        }

        ColorPicker.Color = SelectedColor;
        ColorPicker.ColorChanged += ColorPicker_ColorChanged;

        if (Flyout == null)
        {
            Flyout = new Flyout()
            {
                // TODO: Expose Placement
                Placement = FlyoutPlacementMode.BottomEdgeAlignedLeft,
                FlyoutPresenterStyle = FlyoutPresenterStyle,
                Content = ColorPicker
            };
        }

        if (CheckeredBackgroundBorder != null)
        {
            CheckeredBackgroundBorder.Loaded -= this.CheckeredBackgroundBorder_Loaded;
        }

        CheckeredBackgroundBorder = GetTemplateChild(nameof(CheckeredBackgroundBorder)) as Border;

        if (CheckeredBackgroundBorder != null)
        {
            CheckeredBackgroundBorder.Loaded += this.CheckeredBackgroundBorder_Loaded;
        }
    }

    private static void SelectedColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is ColorPickerButton instance && !(instance.ColorPicker is null))
        {
            instance.ColorPicker.Color = instance.SelectedColor;
        }
    }

    private void ColorPicker_ColorChanged(Microsoft.UI.Xaml.Controls.ColorPicker sender, Microsoft.UI.Xaml.Controls.ColorChangedEventArgs args)
    {
        SelectedColor = args.NewColor;
    }

    private async void CheckeredBackgroundBorder_Loaded(object sender, RoutedEventArgs e)
    {
        if (sender is Border border)
        {
            await ColorPickerRenderingHelpers.UpdateBorderBackgroundWithCheckerAsync(border,
          ColorPicker.CheckerBackgroundColor); // TODO: Check initialization
        }
    }
}
