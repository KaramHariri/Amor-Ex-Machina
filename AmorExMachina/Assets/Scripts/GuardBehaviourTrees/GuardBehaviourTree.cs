public class GuardBehaviourTree
{
    SelectorNode rootNode;

    // KnockedOut
    Disabled disabled;

    // Player in sight
    SequenceNode playerDetection;
    PlayerInSightCheck playerInSightCheck;
    SucceederNode playerInSightSucceeder;
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
    SelectorNode movementCheck;
    SequenceNode stationaryGuard;
    StationaryGuardCheck stationaryGuardCheck;
    SelectorNode stationary;
    StationaryIdle stationaryIdle;
    FollowPath goBackToPosition;
    SelectorNode movingGuard;
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
        playerInSightSucceeder = new SucceederNode();
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
        movementCheck = new SelectorNode();
        stationaryGuard = new SequenceNode();
        stationaryGuardCheck = new StationaryGuardCheck(agent);
        stationary = new SelectorNode();
        goBackToPosition = new FollowPath(agent);
        stationaryIdle = new StationaryIdle(agent);
        movingGuard = new SelectorNode();
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
        playerDetection.AddChild(playerInSightSucceeder);
        playerInSightSucceeder.AddChild(chasePlayer);
        //playerDetection.AddChild(chasePlayer);

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
        rootNode.AddChild(movementCheck);
        movementCheck.AddChild(stationaryGuard);
        stationaryGuard.AddChild(stationaryGuardCheck);
        stationaryGuard.AddChild(stationary);
        stationary.AddChild(stationaryIdle);
        stationary.AddChild(goBackToPosition);
        movementCheck.AddChild(movingGuard);
        movingGuard.AddChild(idle);
        movingGuard.AddChild(followPath);
    }

    public void Run()
    {
        rootNode.Run();
    }
}
