public class GuardBehaviourTree
{
    SelectorNode rootNode;

    // KnockedOut
    Disabled disabled;

    // Player in sight
    // player is in sight
    SelectorNode playerInSight;
    SequenceNode playerDetection;
    PlayerInSightCheck playerInSightCheck;
    SucceederNode playerInSightSucceeder;
    ChasePlayer chasePlayer;
    // player was in sight
    SequenceNode playerWasDetected;
    PlayerWasInSightCheck playerWasInSightCheck;
    SucceederNode playerWasInSightSucceeder;
    SequenceNode playerWasInSight;
    MoveToLastSightPosition moveToLastSightPosition;
    LastSightPositionLookAround lastSightPositionLookAround;

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
    // Distracted.
    SequenceNode distractedSequence;
    DistractionCheck distractedCheck;
    SucceederNode distractedSucceeder;
    SequenceNode distracted;
    InvestigateDistraction moveToDistractionPosition;
    LookAround distractionLookAround;
    
    // Knockedout Guard
    SequenceNode disabledGuard;
    DisabledGuardCheck disabledGuardCheck;
    SucceederNode disabledGuardFoundSucceeder;
    SequenceNode disabledGuardFound;
    MoveToDisabledGuardPosition moveToDisabledGuardPosition;
    EnableGuard enableDisabledGuard;

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
        playerInSight = new SelectorNode();

        playerDetection = new SequenceNode();
        playerInSightCheck = new PlayerInSightCheck(agent);
        playerInSightSucceeder = new SucceederNode();
        chasePlayer = new ChasePlayer(agent);
        // Player was in sight
        playerWasDetected = new SequenceNode();
        playerWasInSightCheck = new PlayerWasInSightCheck(agent);
        playerWasInSightSucceeder = new SucceederNode();
        playerWasInSight = new SequenceNode();
        moveToLastSightPosition = new MoveToLastSightPosition(agent);
        lastSightPositionLookAround = new LastSightPositionLookAround(agent);

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

        // Distracted.
        distractedSequence = new SequenceNode();
        distractedCheck = new DistractionCheck(agent);
        distractedSucceeder = new SucceederNode();
        distracted = new SequenceNode();
        moveToDistractionPosition = new InvestigateDistraction(agent);
        distractionLookAround = new LookAround(agent);

        // Knockedout Guard
        disabledGuard = new SequenceNode();
        disabledGuardCheck = new DisabledGuardCheck(agent);
        disabledGuardFoundSucceeder = new SucceederNode();
        disabledGuardFound = new SequenceNode();
        moveToDisabledGuardPosition = new MoveToDisabledGuardPosition(agent);
        enableDisabledGuard = new EnableGuard(agent);

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
        // Player Was Detected.
        rootNode.AddChild(playerWasDetected);
        playerWasDetected.AddChild(playerWasInSightCheck);
        playerWasDetected.AddChild(playerWasInSightSucceeder);
        playerWasInSightSucceeder.AddChild(playerWasInSight);
        playerWasInSight.AddChild(moveToLastSightPosition);
        playerWasInSight.AddChild(lastSightPositionLookAround);

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

        suspiciousCheck.AddChild(distractedSequence);
        distractedSequence.AddChild(distractedCheck);
        distractedSequence.AddChild(distractedSucceeder);
        distractedSucceeder.AddChild(distracted);
        distracted.AddChild(moveToDistractionPosition);
        distracted.AddChild(distractionLookAround);

        suspiciousCheck.AddChild(disabledGuard);
        disabledGuard.AddChild(disabledGuardCheck);
        disabledGuard.AddChild(disabledGuardFoundSucceeder);
        disabledGuardFoundSucceeder.AddChild(disabledGuardFound);
        disabledGuardFound.AddChild(moveToDisabledGuardPosition);
        disabledGuardFound.AddChild(enableDisabledGuard);

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
