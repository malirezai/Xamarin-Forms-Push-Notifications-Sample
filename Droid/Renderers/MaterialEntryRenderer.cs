using System;
using System.ComponentModel;
using Android.Content;
using Android.Support.Design.Widget;
using Android.Text;
using Android.Views;
using Android.Views.InputMethods;
using Android.Widget;
using pushsample.Controls;
using pushsample.Droid.Helpers;
using pushsample.Droid.Renderers;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using Application = Android.App.Application;
using TextChangedEventArgs = Android.Text.TextChangedEventArgs;
[assembly: ExportRenderer(typeof(MaterialEntry), typeof(MaterialEntryRenderer))]

namespace pushsample.Droid.Renderers
{
	public class MaterialEntryRenderer : Xamarin.Forms.Platform.Android.AppCompat.ViewRenderer<MaterialEntry, TextInputLayout>
	{
		private EditText _defaultEditTextForValues;
		private bool _preventTextLoop;

		protected override void OnElementChanged(ElementChangedEventArgs<MaterialEntry> e)
		{
			base.OnElementChanged(e);

			if (e.OldElement != null)
			{
				// unsubscribe
				//if (Element != null)
				//{
				//	Element.HideKeyboard -= ElementOnHideKeyboard;
				//}
				Control.EditText.KeyPress -= EditTextOnKeyPress;
				Control.EditText.TextChanged -= EditTextOnTextChanged;
			}

			if (e.NewElement != null)
			{
				var ctrl = CreateNativeControl();
				SetNativeControl(ctrl);
				_defaultEditTextForValues = new EditText(Context);

				SetText();
				SetHintText();
				SetTextColor();
				SetBackgroundColor();
				SetHintColor();
				SetIsPassword();
				SetKeyboard();

				// Subscribe
				Control.EditText.TextChanged += EditTextOnTextChanged;
				Control.EditText.KeyPress += EditTextOnKeyPress;
				if (Element != null)
				{
					//Element.HideKeyboard += ElementOnHideKeyboard;
				}
			}
		}

		protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			base.OnElementPropertyChanged(sender, e);

			if (e.PropertyName == Entry.PlaceholderProperty.PropertyName)
			{
				SetHintText();
			}

			if (e.PropertyName == Entry.TextColorProperty.PropertyName)
			{
				SetTextColor();
			}

			if (e.PropertyName == VisualElement.BackgroundColorProperty.PropertyName)
			{
				SetBackgroundColor();
			}

			if (e.PropertyName == Entry.IsPasswordProperty.PropertyName)
			{
				SetIsPassword();
			}

			if (e.PropertyName == Entry.TextProperty.PropertyName)
			{
				SetText();
			}

			if (e.PropertyName == Entry.PlaceholderColorProperty.PropertyName)
			{
				SetHintColor();
			}

			if (e.PropertyName == InputView.KeyboardProperty.PropertyName)
			{
				SetKeyboard();
			}
		}

		private void ElementOnHideKeyboard(object sender, EventArgs eventArgs)
		{
			var manager = (InputMethodManager)Application.Context.GetSystemService(Context.InputMethodService);
			manager.HideSoftInputFromWindow(Control.EditText.WindowToken, 0);
		}

		private void SetIsPassword()
		{
			Control.EditText.InputType = Element.IsPassword
				? InputTypes.TextVariationPassword | InputTypes.ClassText
				: Control.EditText.InputType;
		}

		private void SetBackgroundColor()
		{
			Control.SetBackgroundColor(Element.BackgroundColor.ToAndroid());
		}

		private void SetHintText()
		{
			Control.Hint = Element.Placeholder;
		}

		private void SetHintColor()
		{
			if (Element.PlaceholderColor == Color.Default)
			{
				Control.EditText.SetHintTextColor(_defaultEditTextForValues.HintTextColors);
			}
			else
			{
				Control.EditText.SetHintTextColor(Element.PlaceholderColor.ToAndroid());
			}
		}

		private void SetTextColor()
		{
			if (Element.TextColor == Color.Default)
			{
				Control.EditText.SetTextColor(_defaultEditTextForValues.TextColors);
			}
			else
			{
				Control.EditText.SetTextColor(Element.TextColor.ToAndroid());
			}
		}

		private void SetKeyboard()
		{
			Control.EditText.InputType = Element.Keyboard.ToNative();
		}

		protected override TextInputLayout CreateNativeControl()
		{
			var layout = (TextInputLayout)LayoutInflater.From(Context).Inflate(Resource.Layout.TextInputLayout, null);
			var inner = layout.FindViewById(Resource.Id.textInputLayout);
			if (!string.IsNullOrWhiteSpace(Element.AutomationId))
			{
				inner.ContentDescription = Element.AutomationId;
			}
			return layout;
		}

		private void EditTextOnKeyPress(object sender, KeyEventArgs args)
		{
			args.Handled = args.KeyCode == Keycode.Enter;
			if (args.KeyCode == Keycode.Enter && args.Event.Action == KeyEventActions.Up)
			{
				//Element.OnCompleted(this, EventArgs.Empty);
			}
		}

		private void EditTextOnTextChanged(object sender, TextChangedEventArgs args)
		{
			// As I type: send the EditText to the Forms Entry
			var selection = Control.EditText.SelectionStart;
			if (!_preventTextLoop)
			{
				Element.Text = args.Text.ToString();
			}
			if (Element == null || Element.Text == null) return;

			var index = selection > Element.Text.Length ? Element.Text.Length : selection;
			Control.EditText.SetSelection(index);
		}

		private void SetText()
		{
			_preventTextLoop = true;
			if (Control.EditText.Text != Element.Text)
			{
				// If I programmatically change text on the Forms Entry, 
				// send the forms entry to the native EditText
				Control.EditText.Text = Element.Text;
			}
			_preventTextLoop = false;
		}

		protected override void Dispose(bool disposing)
		{
			// this is here because the PopupPage disposes of the object in a weird way.
			// unsubscribe
			if (Element != null)
			{
				//Element.HideKeyboard -= ElementOnHideKeyboard;
			}
			Control.EditText.KeyPress -= EditTextOnKeyPress;
			Control.EditText.TextChanged -= EditTextOnTextChanged;
			base.Dispose(disposing);
		}
	}
}