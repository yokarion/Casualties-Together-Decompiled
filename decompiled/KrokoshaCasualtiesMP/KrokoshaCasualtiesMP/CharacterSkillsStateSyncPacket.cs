using KrokoshaCasualtiesUtils;

namespace KrokoshaCasualtiesMP;

public struct CharacterSkillsStateSyncPacket : INetSerializeByMemcpy
{
	public ushort skill_exp_STR;

	public ushort skill_exp_RES;

	public ushort skill_exp_INT;

	public ushort skill_STR;

	public ushort skill_RES;

	public ushort skill_INT;

	public CharacterSkillsStateSyncPacket(Body body)
	{
		Skills skills = body.skills;
		skill_exp_STR = (ushort)skills.expSTR;
		skill_exp_RES = (ushort)skills.expRES;
		skill_exp_INT = (ushort)skills.expINT;
		skill_STR = (ushort)skills.STR;
		skill_RES = (ushort)skills.RES;
		skill_INT = (ushort)skills.INT;
	}

	public void Apply(Body body)
	{
		bool num = Util.IsBodyLocal(body);
		Skills skills = body.skills;
		if (num)
		{
			skills.AddExp(0, ((float)(int)skill_exp_STR - skills.expSTR) / Skills.xpGainMult);
			skills.AddExp(1, ((float)(int)skill_exp_RES - skills.expRES) / Skills.xpGainMult);
			skills.AddExp(2, ((float)(int)skill_exp_INT - skills.expINT) / Skills.xpGainMult);
		}
		else
		{
			skills.expSTR = (int)skill_exp_STR;
			skills.expRES = (int)skill_exp_RES;
			skills.expINT = (int)skill_exp_INT;
			skills.CheckForLevelUp(ref skills.STR, skills.expSTR, ref skills.maxSTR);
			skills.CheckForLevelUp(ref skills.INT, skills.expINT, ref skills.maxINT);
			skills.CheckForLevelUp(ref skills.INT, skills.expINT, ref skills.maxINT);
		}
		skills.UpdateExpBoundaries();
		skills.STR = skill_STR;
		skills.RES = skill_RES;
		skills.INT = skill_INT;
	}
}
