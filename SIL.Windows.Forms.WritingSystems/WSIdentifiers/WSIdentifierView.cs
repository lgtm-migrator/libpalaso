using System;
using System.Windows.Forms;
using SIL.WritingSystems;

namespace SIL.Windows.Forms.WritingSystems.WSIdentifiers
{
	public partial class WSIdentifierView : UserControl, ISelectableIdentifierOptions
	{
		private WritingSystemSetupModel _model;

		public WSIdentifierView()
		{
			InitializeComponent();
		}

		public void BindToModel(WritingSystemSetupModel model)
		{
			if (_model != null)
			{
				_model.SelectionChanged -= ModelSelectionChanged;
			}
			_model = model;
			if (_model != null)
			{
				_model.SelectionChanged += ModelSelectionChanged;
			}
			this.Disposed += OnDisposed;
			if (_model.IsSpecialComboLocked)
			{
				switch (_model.LockedSpecialCombo)
				{
					case WritingSystemSetupModel.SelectionsForSpecialCombo.Ipa:
						AddDetailsControl(new IpaIdentifierView(model));
						break;
					case WritingSystemSetupModel.SelectionsForSpecialCombo.Voice:
						AddDetailsControl(new VoiceIdentifierView(model));
						break;
					case WritingSystemSetupModel.SelectionsForSpecialCombo.ScriptRegionVariant:
						AddDetailsControl(new ScriptRegionVariantView(model));
						break;
					default:
						throw new ApplicationException("Special Combo is locked to an unknown selection.");
				}
			}
			else
			{
				AddDetailsControl(new NothingSpecialView(model));
				AddDetailsControl(new IpaIdentifierView(model));
				AddDetailsControl(new VoiceIdentifierView(model));
				AddDetailsControl(new ScriptRegionVariantView(model));
			}
			comboBox1.DisplayMember = "ChoiceName";
			comboBox1.SelectedIndex = 0;
			UpdateFromModel();
		}

		private void AddDetailsControl(Control view)
		{
			view.Dock = DockStyle.Fill;
			//leave invisible for now
			comboBox1.Items.Add(view);
		}

		private void UpdateFromModel()
		{
			if (_model.CurrentDefinition != null)
			{
				this.Enabled = true;
				_abbreviation.Text = _model.CurrentAbbreviation;
//                _name.Text = _model.CurrentLanguageName;
				//_code.Text=_model.CurrentISO;
				if (!_model.IsSpecialComboLocked)
				{
					UpdateSpecialComboBox();
					comboBox1.SelectedIndex = (int)_model.SelectionForSpecialCombo;
				}
			}
			else
			{
				this.Enabled = false;
				_abbreviation.Text = string.Empty;
				_detailPanel.Controls.Clear();
  //              _name.Text = string.Empty;
			   // _code.Text = string.Empty;
				comboBox1.SelectedIndex = 0;
			}
		}

		private void UpdateSpecialComboBox()
		{
			if (_model.CurrentIso == WellKnownSubtags.UnlistedLanguage)
			{
				if (comboBox1.Items.Count == 4)
				{
					AddDetailsControl(new UnlistedLanguageView(_model));
				}
			}
			else
			{
				if (comboBox1.Items.Count == 5)
				{
					comboBox1.Items.RemoveAt(4);
				}
			}
		}

		private void ModelSelectionChanged(object sender, EventArgs e)
		{
			UpdateFromModel();
		}

		void OnDisposed(object sender, EventArgs e)
		{
			if (_model != null)
				_model.SelectionChanged -= ModelSelectionChanged;
		}

		private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (comboBox1.SelectedItem == null || _model.CurrentDefinition==null)
				return;

			_detailPanel.Controls.Clear();
			_detailPanel.Controls.Add((Control)comboBox1.SelectedItem);
			((ISelectableIdentifierOptions)comboBox1.SelectedItem).Selected();
		}

		private void OnVisibleChanged(object sender, EventArgs e)
		{
			//UpdateSpecialComboBox();

		}

		public void Selected()
		{
			comboBox1_SelectedIndexChanged(null, null);
		}

		private void _abbreviation_TextChanged(object sender, EventArgs e)
		{
			var s = _abbreviation.Text.Trim();
			if(s.Length > 0  && s!= _model.CurrentAbbreviation)
			{
				_model.CurrentAbbreviation = s;
			}
		}

		public void MoveDataFromViewToModel()
		{
			((ISelectableIdentifierOptions)comboBox1.SelectedItem).MoveDataFromViewToModel();
		}

		public void UnwireBeforeClosing()
		{
			((ISelectableIdentifierOptions)comboBox1.SelectedItem).UnwireBeforeClosing();
		}
	}

	public interface ISelectableIdentifierOptions
	{
		void Selected();
		void MoveDataFromViewToModel();
		void UnwireBeforeClosing();
	}
}