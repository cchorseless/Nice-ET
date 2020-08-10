﻿namespace NiceET
{
    public static class MailBoxComponentHelper
    {
		public static async ETTask Handle(this MailBoxComponent self, Session session, IActorMessage message)
		{
			using (await CoroutineLockComponent.Instance.Wait(CoroutineLockType.Mailbox, message.ActorId))
			{
				switch (self.MailboxType)
				{
					case MailboxType.GateSession:
						IActorMessage iActorMessage = message as IActorMessage;
						// 发送给客户端
						Session clientSession = self.Parent as Session;
						iActorMessage.ActorId = 0;
						clientSession.Send(iActorMessage);
						break;
					case MailboxType.MessageDispatcher:
						await ActorMessageDispatcherComponent.Instance.Handle(self.Parent, session, message);
						break;
					case MailboxType.UnOrderMessageDispatcher:
						self.HandleInner(session, message).Coroutine();
						break;
				}
			}
		}

		private static async ETVoid HandleInner(this MailBoxComponent self, Session session, IActorMessage message)
		{
			await ActorMessageDispatcherComponent.Instance.Handle(self.Parent, session, message);
		}
	}
}
