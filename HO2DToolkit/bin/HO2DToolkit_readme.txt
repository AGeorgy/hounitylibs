REQUIREMENTS - CODE
------------------------------------------------

||||| 2D Toolkit

- tk2dBaseSprite
	add ", Holoville.HO2DToolkit.IHOtk2dSprite"
	add "public string GetSpriteName() { return CurrentSprite.name; }"
- tk2dSlicedSprite
	add ", Holoville.HO2DToolkit.IHOtk2dSlicedSprite"
- tk2dTextMesh
	add ", Holoville.HO2DToolkit.IHOtk2dTextMesh"
	and "public void SetMaxChars(int val) { _maxChars = val; }"
	Change text.set to:
	"set 
	{
		if(value == null) value = "";
		if(!value.Equals(data.text)) {
			UpgradeData();
			data.text = value;
			SetNeedUpdate(UpdateFlags.UpdateText);
		}
	}"