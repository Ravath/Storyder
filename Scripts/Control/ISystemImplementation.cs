using System;

namespace Storyder;

public interface ISystemImplementation
{
    void Init();
    bool IsGameFinished { get; }
    void DoEffectChecks(StoryReader storyReader);
    StoryderEffect ParseEffectCommand(string commandName, string[] arguments);
}
