DROP TABLE IF EXISTS  OccurrenceNotes
DROP TABLE IF EXISTS  Occurrences
DROP TABLE IF EXISTS  Patterns
DROP TABLE IF EXISTS  PatternTypes
DROP TABLE IF EXISTS  PitchBendItems
DROP TABLE IF EXISTS  ChordOccurrences
DROP TABLE IF EXISTS  MelodyNotes
DROP TABLE IF EXISTS  Melodies
DROP TABLE IF EXISTS  Chords
DROP TABLE IF EXISTS  Notes
DROP TABLE IF EXISTS  PatternTypes
DROP TABLE IF EXISTS  RythmPatterns
DROP TABLE IF EXISTS  SongSimplifications
DROP TABLE IF EXISTS  TempoChanges
DROP TABLE IF EXISTS  Bars
DROP TABLE IF EXISTS  SongStats
DROP TABLE IF EXISTS  Songs
DROP TABLE IF EXISTS  Bands
DROP TABLE IF EXISTS  Styles
DROP TABLE IF EXISTS  TimeSignatures

CREATE TABLE TimeSignatures(
	Id bigint IDENTITY(1,1) primary key clustered NOT NULL,
	Numerator int NOT NULL,
	Denominator int NOT NULL,
)


SET IDENTITY_INSERT TimeSignatures ON

insert into TimeSignatures(Id, Numerator, Denominator)
values (1, 4, 4)

insert into TimeSignatures(Id, Numerator, Denominator)
values (2, 3, 4)

insert into TimeSignatures(Id, Numerator, Denominator)
values (3, 2, 2)

insert into TimeSignatures(Id, Numerator, Denominator)
values (4, 6, 8)

insert into TimeSignatures(Id, Numerator, Denominator)
values (5, 12, 8)

insert into TimeSignatures(Id, Numerator, Denominator)
values (6, 7, 4)

insert into TimeSignatures(Id, Numerator, Denominator)
values (7, 2, 4)

insert into TimeSignatures(Id, Numerator, Denominator)
values (8, 5, 4)

insert into TimeSignatures(Id, Numerator, Denominator)
values (9, 9, 8)

insert into TimeSignatures(Id, Numerator, Denominator)
values (10, 3, 8)

insert into TimeSignatures(Id, Numerator, Denominator)
values (11, 9, 16)

insert into TimeSignatures(Id, Numerator, Denominator)
values (12, 6, 4)

insert into TimeSignatures(Id, Numerator, Denominator)
values (13, 8, 8)

insert into TimeSignatures(Id, Numerator, Denominator)
values (14, 12, 16)

insert into TimeSignatures(Id, Numerator, Denominator)
values (15, 3, 2)

insert into TimeSignatures(Id, Numerator, Denominator)
values (16, 6, 16)

insert into TimeSignatures(Id, Numerator, Denominator)
values (17, 4, 2)

insert into TimeSignatures(Id, Numerator, Denominator)
values (18, 4, 8)

insert into TimeSignatures(Id, Numerator, Denominator)
values (19, 7, 8)

insert into TimeSignatures(Id, Numerator, Denominator)
values (20, 13, 8)

insert into TimeSignatures(Id, Numerator, Denominator)
values (21, 12, 4)

insert into TimeSignatures(Id, Numerator, Denominator)
values (22, 8, 4)
SET IDENTITY_INSERT TimeSignatures OFF

CREATE TABLE Styles(
	Id bigint IDENTITY(1,1) primary key clustered NOT NULL,
	[Name] nvarchar(60) NULL,
)
ALTER TABLE Styles ADD CONSTRAINT UC_Styles UNIQUE (Name);

SET IDENTITY_INSERT Styles ON

insert into Styles(Id, [Name])
values (1, 'Classic')

insert into Styles(Id, [Name])
values (2, 'Rock')

insert into Styles(Id, [Name])
values (3, 'Jazz')

SET IDENTITY_INSERT Styles OFF

CREATE TABLE Bands(
	Id bigint IDENTITY(1,1) primary key clustered NOT NULL,
	[Name] nvarchar(100) NOT NULL,
	StyleId bigint NOT NULL,
)
ALTER TABLE Bands ADD CONSTRAINT UC_Bands UNIQUE (Name);

ALTER TABLE Bands  WITH CHECK ADD  CONSTRAINT FK_Bands_Styles FOREIGN KEY(StyleId)
REFERENCES Styles (Id)
GO

ALTER TABLE Bands CHECK CONSTRAINT FK_Bands_Styles
GO

create table SongStats(
	Id bigint IDENTITY(1,1) primary key clustered NOT NULL,
    SongId bigint not null,
	DurationInSeconds bigint null,
	HasMoreThanOneChannelPerTrack bit NULL,
	HasMoreThanOneInstrumentPerTrack bit NULL,
	HighestPitch bigint null,
	LowestPitch bigint null,
	NumberBars bigint null,
	NumberOfTicks bigint null,
	TempoInBeatsPerMinute bigint null,
	TempoInMicrosecondsPerBeat bigint null,
	TimeSignatureId bigint NULL,
	TotalDifferentPitches bigint null,
	TotalUniquePitches bigint null,
	TotalTracks bigint null,
    TotalTracksWithoutNotes bigint null,
    TotalBassTracks bigint null,
	TotalChordTracks bigint null,
	TotalMelodicTracks bigint null,
    TotalPercussionTracks bigint null,
	TotalInstruments bigint null,
    InstrumentsAsString nvarchar(400)  null,
	TotalPercussionInstruments bigint null,
    TotalChannels bigint null,
	TotalTempoChanges bigint null,
    TotalEvents bigint null,
	TotalNoteEvents bigint null,
	TotalPitchBendEvents bigint null,
	TotalControlChangeEvents bigint null,
    TotalProgramChangeEvents bigint null,
	TotalSustainPedalEvents bigint null,
	TotalChannelIndependentEvents bigint null
)



CREATE TABLE Songs(
	Id bigint IDENTITY(1,1) primary key clustered NOT NULL,
	[Name] nvarchar(500) NOT NULL,
	BandId bigint NULL,
	StyleId bigint NOT NULL,
	MidiBase64Encoded nvarchar(max) NOT NULL
)


ALTER TABLE Songs  WITH CHECK ADD  CONSTRAINT FK_Songs_Bands FOREIGN KEY(BandId)
REFERENCES Bands (Id)
GO

ALTER TABLE Songs CHECK CONSTRAINT FK_Songs_Bands
GO

ALTER TABLE Songs  WITH CHECK ADD  CONSTRAINT FK_Songs_Styles FOREIGN KEY(StyleId)
REFERENCES Styles (Id)
GO

ALTER TABLE Songs CHECK CONSTRAINT FK_Songs_Styles
GO

ALTER TABLE SongStats  WITH CHECK ADD  CONSTRAINT FK_SongStats_TimeSignatures FOREIGN KEY(TimeSignatureId)
REFERENCES TimeSignatures (Id)
GO

ALTER TABLE SongStats CHECK CONSTRAINT FK_SongStats_TimeSignatures
GO

CREATE TABLE SongSimplifications(
	Id bigint IDENTITY(1,1) primary key clustered NOT NULL,
	SimplificationVersion bigint not null,
	SongId bigint not null,
    NumberOfVoices bigint not null
) 

CREATE TABLE Chords(
    Id bigint IDENTITY(1,1) primary key clustered NOT NULL,
    PitchesAsString varchar(200) not null,
    PitchLettersAsString varchar(100) not null,
    IntervalsAsString varchar(100) not null
)
ALTER TABLE Chords ADD CONSTRAINT UC_PitchesAsString UNIQUE (PitchesAsString)

CREATE TABLE Melodies(
    Id bigint IDENTITY(1,1) primary key clustered NOT NULL,
	SongSimplificationId bigint not null,
    Instrument tinyInt not null,
    Voice tinyInt not null
)
ALTER TABLE Melodies  WITH CHECK ADD  CONSTRAINT FK_Melodies_SongSimplification FOREIGN KEY(SongSimplificationId)
REFERENCES SongSimplifications (Id)


CREATE TABLE ChordOccurrences(
    Id bigint IDENTITY(1,1) primary key clustered NOT NULL,
    ChordId bigint not null,
	SongSimplificationId bigint not null,
    StartTick bigint not null,
    EndTick bigint not null
)
ALTER TABLE ChordOccurrences  WITH CHECK ADD  CONSTRAINT FK_Chord_ChordOccurrences FOREIGN KEY(ChordId)
REFERENCES Chords (Id)

CREATE TABLE Notes(
	Id bigint IDENTITY(1,1) primary key clustered NOT NULL,
	Pitch tinyint NOT NULL,
	Volume tinyint NOT NULL,
	StartSinceBeginningOfSongInTicks bigint NOT NULL,
	EndSinceBeginningOfSongInTicks bigint NOT NULL,
	Instrument tinyint NOT NULL,
	IsPercussion bit null,
	Voice tinyint not null,
	SongSimplificationId bigint not null,
    ChordId bigint null
)
ALTER TABLE Notes  WITH CHECK ADD  CONSTRAINT FK_Notes_SongSimplifications FOREIGN KEY(SongSimplificationId)
REFERENCES SongSimplifications (Id)

CREATE TABLE MelodyNotes(
    Id bigint IDENTITY(1,1) primary key clustered NOT NULL,
    MelodyId bigint not null,
    NoteId bigint not null
)
ALTER TABLE MelodyNotes  WITH CHECK ADD  CONSTRAINT FK_MelodyNotes_Melodies FOREIGN KEY(MelodyId)
REFERENCES Melodies (Id)
ALTER TABLE MelodyNotes  WITH CHECK ADD  CONSTRAINT FK_MelodyNotes_Notes FOREIGN KEY(NoteId)
REFERENCES Notes (Id)


CREATE TABLE Bars(
	Id bigint IDENTITY(1,1) primary key clustered NOT NULL,
	BarNumber bigint null,
	TicksFromBeginningOfSong bigint NULL,
	TimeSignatureId bigint NULL,
	HasTriplets bit NULL,
	TempoInMicrosecondsPerQuarterNote bigint null,
	SongId bigint not null
)
ALTER TABLE Bars  WITH CHECK ADD  CONSTRAINT FK_Bars_TimeSignatures FOREIGN KEY(TimeSignatureId)
REFERENCES TimeSignatures (Id)

ALTER TABLE Bars  WITH CHECK ADD  CONSTRAINT FK_Bars_Songs FOREIGN KEY(SongId)
REFERENCES Songs (Id)

CREATE TABLE PitchBendItems(
	Id bigint IDENTITY(1,1) primary key clustered NOT NULL,
	TicksSinceBeginningOfSong bigint NULL,
	Pitch int null,
	NoteId bigint not null
) 
ALTER TABLE PitchBendItems  WITH CHECK ADD  CONSTRAINT FK_PitchBendItems_Notes FOREIGN KEY(NoteId)
REFERENCES Notes (Id)



CREATE TABLE TempoChanges(
	Id bigint IDENTITY(1,1) primary key clustered NOT NULL,
	TicksSinceBeginningOfSong bigint NULL,
	MicrosecondsPerQuarterNote bigint null,
	SongId bigint not null
) 
ALTER TABLE TempoChanges  WITH CHECK ADD  CONSTRAINT FK_TempoChanges_Songs FOREIGN KEY(SongId)
REFERENCES Songs (Id)

create table PatternTypes(
	Id tinyint primary key clustered NOT NULL,
	TypeName varchar(10) not null
)

insert into PatternTypes(Id, TypeName)
values (1, 'Pitch')
insert into PatternTypes(Id, TypeName)
values (2, 'Rythm')
insert into PatternTypes(Id, TypeName)
values (3, 'Melody')
     

CREATE TABLE Patterns(
	Id bigint IDENTITY(1,1) primary key clustered NOT NULL,
	AsString varchar(600) not null,
	PatternTypeId tinyint not null,
	CONSTRAINT IX_UniquePatterns UNIQUE (AsString, PatternTypeId)
) 
ALTER TABLE Patterns  WITH CHECK ADD  CONSTRAINT FK_Patterns_PatternTypes FOREIGN KEY(PatternTypeId)
REFERENCES PatternTypes (Id)

CREATE TABLE Occurrences(
	Id bigint IDENTITY(1,1) primary key clustered NOT NULL,
	SongSimplificationId bigint not null,
	PatternId bigint not null
) 
ALTER TABLE Occurrences  WITH CHECK ADD  CONSTRAINT FK_Occurrences_SongSimplifications FOREIGN KEY(SongSimplificationId)
REFERENCES SongSimplifications (Id)
ALTER TABLE Occurrences  WITH CHECK ADD  CONSTRAINT FK_Occurrences_Patterns FOREIGN KEY(PatternId)
REFERENCES Patterns (Id)


CREATE TABLE OccurrenceNotes(
    Id bigint IDENTITY(1,1) primary key clustered NOT NULL,
    OccurrenceId bigint not null,
    NoteId bigint not null
)
ALTER TABLE OccurrenceNotes  WITH CHECK ADD  CONSTRAINT FK_OccurrenceNotes_Occurrences FOREIGN KEY(OccurrenceId)
REFERENCES Occurrences (Id)
ALTER TABLE OccurrenceNotes  WITH CHECK ADD  CONSTRAINT FK_OccurrenceNotes_Notes FOREIGN KEY(NoteId)
REFERENCES Notes (Id)
