using System.Collections;

namespace StudySmortAPI.Model;

public record struct UserData(string Id, string Token);
public record NotebookData(string Id,ICollection<string> Pages, string ParentId);
public record FolderData(string Id,string Name, string? ParentId, string? ParentName, ICollection<FolderData> ChildFolders, ICollection<NotebookData> ChildNotebooks);

public record struct DeadlineData(string Id, DateTime DateTimeUtc ,string Note, string Title);

public record FlashCardData(string Id,string Word,string Translation,int CorrectCnt,int WrongCnt,int SkipCnt, string CategoryId, string CategoryName);
public record struct FlashcardCategoryData(string Id,string Name);