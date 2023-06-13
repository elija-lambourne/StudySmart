using System.Collections;

namespace StudySmortAPI.Model;

public record struct UserData(Guid Id, string JwtToken);
public record NotebookData(Guid Id,ICollection<string> Pages, Guid ParentId);
public record FolderData(Guid Id,string Name, Guid ParentId, string ParentName, ICollection<FolderData> ChildFolders, ICollection<NotebookData> ChildNotebooks);

public record struct DeadlineData(Guid Id, DateTime DateTimeUtc ,string Note, string Title);

public record FlashCardData(Guid Id,string Word,string Translation,int CorrectCnt,int WrongCnt,int SkipCnt, Guid CategoryId, string CategoryName);
public record struct FlashcardCategoryData(Guid Id,string Name);