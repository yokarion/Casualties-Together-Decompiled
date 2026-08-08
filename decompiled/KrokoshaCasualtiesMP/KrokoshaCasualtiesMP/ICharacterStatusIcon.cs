using UnityEngine;

namespace KrokoshaCasualtiesMP;

public interface ICharacterStatusIcon
{
	void Create(CharStatusVisuals visual, SpriteRenderer spr);

	bool AppearCondition();

	bool DisappearCondition();

	void Update();
}
