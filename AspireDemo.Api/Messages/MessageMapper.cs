using AspireDemo.Data.Entities;
using Riok.Mapperly.Abstractions;

namespace AspireDemo.Api.Messages;

[Mapper]
public static partial class MessageMapper
{
    public static partial List<MessageDto> ToDto(this List<Message> message);
    
    [MapProperty(nameof(Message.Timestamp), nameof(MessageDto.Timestamp), StringFormat = "HH:mm")]
    public static partial MessageDto ToDto(this Message message);
}