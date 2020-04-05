public class GuardBehaviourTree
{
    SelectorNode rootNode;

    // KnockedOut
    Disabled disabled;

    // Player in sight
    SequenceNode playerDetection;
    PlayerInSightCheck playerInSightCheck;
    ChasePlayer chasePlayer;

    // Assist
    SequenceNode assistCheck;
    BackupRequestCheck backupRequestCheck;
    SucceederNode assistSucceeder;
    SequenceNode assist;
    MoveToAssistPosition moveToAssistPosition;
    LookAround assistLookAround;

    // Suspicious
    // Hearing.
    SelectorNode suspiciousCheck;
    SequenceNode hearing;
    HearingSensing hearingSensing;
    SucceederNode suspicious;
    SequenceNode investigate;
    Investigate moveToInvestigationPosition;
    LookAround lookAround;
    // Knockedout Guard
    SequenceNode disabledGuard;
    DisabledGuardCheck disabledGuardCheck;
    SucceederNode disabledGuardFoundSucceeder;
    SequenceNode disabledGuardFound;
    MoveToDisabledGuardPosition moveToDisabledGuardPosition;
    EnableGuard wakeUpDisabledGuard;

    // Patrol
    SelectorNode patrol;
    SequenceNode InteractWithTheOtherGuard;
    TalkToOtherGuardCheck talkToOtherGuardCheck;
    SucceederNode talkToOtherGuardSucceeder;
    TalkToOtherGuard talkToOtherGuard;
    Idle idle;
    FollowPath followPath;

    public GuardBehaviourTree(Guard agent)
    {
        CreateNodes(agent);
        ConstructTree();
    }

    void CreateNodes(Guard agent)
    {
        rootNode = new SelectorNode();

        // KnockedOut
        disabled = new Disabled(agent);

        // Player in sight
        playerDetection = new SequenceNode();
        playerInSightCheck = new PlayerInSightCheck(agent);
        chasePlayer = new ChasePlayer(agent);

        // Assist
        assistCheck = new SequenceNode();
        backupRequestCheck = new BackupRequestCheck(agent);
        assistSucceeder = new SucceederNode();
        assist = new SequenceNode();
        moveToAssistPosition = new MoveToAssistPosition(agent);
        assistLookAround = new LookAround(agent);

        // Suspicious
        // Hearing.
        suspiciousCheck = new SelectorNode();
        hearing = new SequenceNode();
        hearingSensing = new HearingSensing(agent);
        suspicious = new SucceederNode();
        investigate = new SequenceNode();
        moveToInvestigationPosition = new Investigate(agent);
        lookAround = new LookAround(agent);
        // Knockedout Guard
        disabledGuard = new SequenceNode();
        disabledGuardCheck = new DisabledGuardCheck(agent);
        disabledGuardFoundSucceeder = new SucceederNode();
        disabledGuardFound = new SequenceNode();
        moveToDisabledGuardPosition = new MoveToDisabledGuardPosition(agent);
        wakeUpDisabledGuard = new EnableGuard(agent);

        // Patrol
        patrol = new SelectorNode();
        InteractWithTheOtherGuard = new SequenceNode();
        talkToOtherGuardCheck = new TalkToOtherGuardCheck(agent);
        talkToOtherGuardSucceeder = new SucceederNode();
        talkToOtherGuard = new TalkToOtherGuard(agent);
        idle = new Idle(agent);
        followPath = new FollowPath(agent);
    }

    void ConstructTree()
    {
        // KnockedOut
        rootNode.AddChild(disabled);

        // Player Detected
        rootNode.AddChild(playerDetection);
        playerDetection.AddChild(playerInSightCheck);
        playerDetection.AddChild(chasePlayer);

        // Assist
        rootNode.AddChild(assistCheck);
        assistCheck.AddChild(backupRequestCheck);
        assistCheck.AddChild(assistSucceeder);
        assistSucceeder.AddChild(assist);
        assist.AddChild(moveToAssistPosition);
        assist.AddChild(assistLookAround);

        // Suspicious
        rootNode.AddChild(suspiciousCheck);

        suspiciousCheck.AddChild(hearing);
        hearing.AddChild(hearingSensing);
        hearing.AddChild(suspicious);
        suspicious.AddChild(investigate);
        investigate.AddChild(moveToInvestigationPosition);
        investigate.AddChild(lookAround);

        suspiciousCheck.AddChild(disabledGuard);
        disabledGuard.AddChild(disabledGuardCheck);
        disabledGuard.AddChild(disabledGuardFoundSucceeder);
        disabledGuardFoundSucceeder.AddChild(disabledGuardFound);
        disabledGuardFound.AddChild(moveToDisabledGuardPosition);
        disabledGuardFound.AddChild(wakeUpDisabledGuard);

        // Patrol
        rootNode.AddChild(patrol);
        patrol.AddChild(InteractWithTheOtherGuard);
        patrol.AddChild(idle);
        patrol.AddChild(followPath);
        InteractWithTheOtherGuard.AddChild(talkToOtherGuardCheck);
        InteractWithTheOtherGuard.AddChild(talkToOtherGuardSucceeder);
        talkToOtherGuardSucceeder.AddChild(talkToOtherGuard);
    }

    public void Run()
    {
        rootNode.Run();
    }
}
