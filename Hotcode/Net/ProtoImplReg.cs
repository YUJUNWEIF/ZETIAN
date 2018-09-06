using System;
using System.Collections.Generic;
using Net;

namespace geniusbaby.pps
{
    public class ProtoImplReg
    {
        public static void Register()
        {
            try 
            {
            	ProtoManager.manager.Register(PID.LockStep, typeof(Net.LockStep), (p, s) => new LockStepImpl(p, s));
            	ProtoManager.manager.Register(PID.LockStepReconnect, typeof(Net.LockStepReconnect), (p, s) => new LockStepReconnectImpl(p, s));

            	ProtoManager.manager.Register(PID.AccountRegisterResponse, typeof(AccountRegisterResponse), (p, s) => new AccountRegisterResponseImpl(p, s));
            	ProtoManager.manager.Register(PID.AccountLoginResponse, typeof(AccountLoginResponse), (p, s) => new AccountLoginResponseImpl(p, s));
            	ProtoManager.manager.Register(PID.PlayerCreateResponse, typeof(PlayerCreateResponse), (p, s) => new PlayerCreateResponseImpl(p, s));
            	ProtoManager.manager.Register(PID.TimeNotify, typeof(TimeNotify), (p, s) => new TimeNotifyImpl(p, s));
            	ProtoManager.manager.Register(PID.VersionCheckResponse, typeof(VersionCheckResponse), (p, s) => new VersionCheckResponseImpl(p, s));
            	ProtoManager.manager.Register(PID.PlayerNotify, typeof(PlayerNotify), (p, s) => new PlayerNotifyImpl(p, s));
            	ProtoManager.manager.Register(PID.TokenNotify, typeof(TokenNotify), (p, s) => new TokenNotifyImpl(p, s));
            	ProtoManager.manager.Register(PID.ArchiveNotify, typeof(ArchiveNotify), (p, s) => new ArchiveNotifyImpl(p, s));
            	ProtoManager.manager.Register(PID.ArchiveResponse, typeof(ArchiveResponse), (p, s) => new ArchiveResponseImpl(p, s));
            	ProtoManager.manager.Register(PID.GuideNotify, typeof(GuideNotify), (p, s) => new GuideNotifyImpl(p, s));
            	ProtoManager.manager.Register(PID.GuideFinishResponse, typeof(GuideFinishResponse), (p, s) => new GuideFinishResponseImpl(p, s));
            	ProtoManager.manager.Register(PID.KnowledgeNotify, typeof(KnowledgeNotify), (p, s) => new KnowledgeNotifyImpl(p, s));
            	ProtoManager.manager.Register(PID.KnowledgeSwitchResponse, typeof(KnowledgeSwitchResponse), (p, s) => new KnowledgeSwitchResponseImpl(p, s));
            	ProtoManager.manager.Register(PID.ClassFinishResponse, typeof(ClassFinishResponse), (p, s) => new ClassFinishResponseImpl(p, s));
            	ProtoManager.manager.Register(PID.WordQueryResponse, typeof(WordQueryResponse), (p, s) => new WordQueryResponseImpl(p, s));
            	ProtoManager.manager.Register(PID.PetNotify, typeof(PetNotify), (p, s) => new PetNotifyImpl(p, s));
            	ProtoManager.manager.Register(PID.PetRmvNotify, typeof(PetRmvNotify), (p, s) => new PetRmvNotifyImpl(p, s));
            	ProtoManager.manager.Register(PID.PetEquipResponse, typeof(PetEquipResponse), (p, s) => new PetEquipResponseImpl(p, s));
            	ProtoManager.manager.Register(PID.PetEggHatchResponse, typeof(PetEggHatchResponse), (p, s) => new PetEggHatchResponseImpl(p, s));
            	ProtoManager.manager.Register(PID.PetMergeResponse, typeof(PetMergeResponse), (p, s) => new PetMergeResponseImpl(p, s));
            	ProtoManager.manager.Register(PID.PetLvUpResponse, typeof(PetLvUpResponse), (p, s) => new PetLvUpResponseImpl(p, s));
            	ProtoManager.manager.Register(PID.PetExplorerSlotNotify, typeof(PetExplorerSlotNotify), (p, s) => new PetExplorerSlotNotifyImpl(p, s));
            	ProtoManager.manager.Register(PID.PetExplorerResponse, typeof(PetExplorerResponse), (p, s) => new PetExplorerResponseImpl(p, s));
            	ProtoManager.manager.Register(PID.PetExplorerGetResponse, typeof(PetExplorerGetResponse), (p, s) => new PetExplorerGetResponseImpl(p, s));
            	ProtoManager.manager.Register(PID.PetExplorerSlotUnlockResponse, typeof(PetExplorerSlotUnlockResponse), (p, s) => new PetExplorerSlotUnlockResponseImpl(p, s));
            	ProtoManager.manager.Register(PID.PetExplorerSlotUseResponse, typeof(PetExplorerSlotUseResponse), (p, s) => new PetExplorerSlotUseResponseImpl(p, s));
            	ProtoManager.manager.Register(PID.ItemNotify, typeof(ItemNotify), (p, s) => new ItemNotifyImpl(p, s));
            	ProtoManager.manager.Register(PID.ItemRmvNotify, typeof(ItemRmvNotify), (p, s) => new ItemRmvNotifyImpl(p, s));
            	ProtoManager.manager.Register(PID.ItemSellResponse, typeof(ItemSellResponse), (p, s) => new ItemSellResponseImpl(p, s));
            	ProtoManager.manager.Register(PID.ItemUseResponse, typeof(ItemUseResponse), (p, s) => new ItemUseResponseImpl(p, s));
            	ProtoManager.manager.Register(PID.ItemSortResponse, typeof(ItemSortResponse), (p, s) => new ItemSortResponseImpl(p, s));
            	ProtoManager.manager.Register(PID.TitleNotify, typeof(TitleNotify), (p, s) => new TitleNotifyImpl(p, s));
            	ProtoManager.manager.Register(PID.TitleEquipResponse, typeof(TitleEquipResponse), (p, s) => new TitleEquipResponseImpl(p, s));
            	ProtoManager.manager.Register(PID.TitleUnlockResponse, typeof(TitleUnlockResponse), (p, s) => new TitleUnlockResponseImpl(p, s));
            	ProtoManager.manager.Register(PID.TitleLvUpResponse, typeof(TitleLvUpResponse), (p, s) => new TitleLvUpResponseImpl(p, s));
            	ProtoManager.manager.Register(PID.ChatNotify, typeof(ChatNotify), (p, s) => new ChatNotifyImpl(p, s));
            	ProtoManager.manager.Register(PID.ChatSendResponse, typeof(ChatSendResponse), (p, s) => new ChatSendResponseImpl(p, s));
            	ProtoManager.manager.Register(PID.GMapChatStatusNotify, typeof(GMapChatStatusNotify), (p, s) => new GMapChatStatusNotifyImpl(p, s));
            	ProtoManager.manager.Register(PID.GMapChatAudioSendNotify, typeof(GMapChatAudioSendNotify), (p, s) => new GMapChatAudioSendNotifyImpl(p, s));
            	ProtoManager.manager.Register(PID.GMapChatTextSendNotify, typeof(GMapChatTextSendNotify), (p, s) => new GMapChatTextSendNotifyImpl(p, s));
            	ProtoManager.manager.Register(PID.MailNotify, typeof(MailNotify), (p, s) => new MailNotifyImpl(p, s));
            	ProtoManager.manager.Register(PID.MailRemoveNotify, typeof(MailRemoveNotify), (p, s) => new MailRemoveNotifyImpl(p, s));
            	ProtoManager.manager.Register(PID.MailSendResponse, typeof(MailSendResponse), (p, s) => new MailSendResponseImpl(p, s));
            	ProtoManager.manager.Register(PID.MailOpenResponse, typeof(MailOpenResponse), (p, s) => new MailOpenResponseImpl(p, s));
            	ProtoManager.manager.Register(PID.MailDeleteResponse, typeof(MailDeleteResponse), (p, s) => new MailDeleteResponseImpl(p, s));
            	ProtoManager.manager.Register(PID.MailGetAttachmentResponse, typeof(MailGetAttachmentResponse), (p, s) => new MailGetAttachmentResponseImpl(p, s));
            	ProtoManager.manager.Register(PID.FriendNotify, typeof(FriendNotify), (p, s) => new FriendNotifyImpl(p, s));
            	ProtoManager.manager.Register(PID.FriendRemoveNotify, typeof(FriendRemoveNotify), (p, s) => new FriendRemoveNotifyImpl(p, s));
            	ProtoManager.manager.Register(PID.FriendAddResponse, typeof(FriendAddResponse), (p, s) => new FriendAddResponseImpl(p, s));
            	ProtoManager.manager.Register(PID.FriendRemoveResponse, typeof(FriendRemoveResponse), (p, s) => new FriendRemoveResponseImpl(p, s));
            	ProtoManager.manager.Register(PID.FriendAcceptResponse, typeof(FriendAcceptResponse), (p, s) => new FriendAcceptResponseImpl(p, s));
            	ProtoManager.manager.Register(PID.FriendGiveResponse, typeof(FriendGiveResponse), (p, s) => new FriendGiveResponseImpl(p, s));
            	ProtoManager.manager.Register(PID.FriendVisitResponse, typeof(FriendVisitResponse), (p, s) => new FriendVisitResponseImpl(p, s));
            	ProtoManager.manager.Register(PID.FriendRobberyResponse, typeof(FriendRobberyResponse), (p, s) => new FriendRobberyResponseImpl(p, s));
            	ProtoManager.manager.Register(PID.FriendChatResponse, typeof(FriendChatResponse), (p, s) => new FriendChatResponseImpl(p, s));
            	ProtoManager.manager.Register(PID.FriendChatNotify, typeof(FriendChatNotify), (p, s) => new FriendChatNotifyImpl(p, s));
            	ProtoManager.manager.Register(PID.ShopNotify, typeof(ShopNotify), (p, s) => new ShopNotifyImpl(p, s));
            	ProtoManager.manager.Register(PID.ShopResponse, typeof(ShopResponse), (p, s) => new ShopResponseImpl(p, s));
            	ProtoManager.manager.Register(PID.ShopBuyResponse, typeof(ShopBuyResponse), (p, s) => new ShopBuyResponseImpl(p, s));
            	ProtoManager.manager.Register(PID.AchieveNotify, typeof(AchieveNotify), (p, s) => new AchieveNotifyImpl(p, s));
            	ProtoManager.manager.Register(PID.AchieveGetAwardResponse, typeof(AchieveGetAwardResponse), (p, s) => new AchieveGetAwardResponseImpl(p, s));
            	ProtoManager.manager.Register(PID.GSrvPvpResponse, typeof(GSrvPvpResponse), (p, s) => new GSrvPvpResponseImpl(p, s));
            	ProtoManager.manager.Register(PID.GSrvPvpNotify, typeof(GSrvPvpNotify), (p, s) => new GSrvPvpNotifyImpl(p, s));
            	ProtoManager.manager.Register(PID.GMapPvpQuitResponse, typeof(GMapPvpQuitResponse), (p, s) => new GMapPvpQuitResponseImpl(p, s));
            	ProtoManager.manager.Register(PID.GMapPvpNotify, typeof(GMapPvpNotify), (p, s) => new GMapPvpNotifyImpl(p, s));
            	ProtoManager.manager.Register(PID.GMapPvpRmvNotify, typeof(GMapPvpRmvNotify), (p, s) => new GMapPvpRmvNotifyImpl(p, s));
            	ProtoManager.manager.Register(PID.GMapPvpConfirmNotify, typeof(GMapPvpConfirmNotify), (p, s) => new GMapPvpConfirmNotifyImpl(p, s));
            	ProtoManager.manager.Register(PID.GMapPvpNoSessionNotify, typeof(GMapPvpNoSessionNotify), (p, s) => new GMapPvpNoSessionNotifyImpl(p, s));
            	ProtoManager.manager.Register(PID.GMapLinkInfoNotify, typeof(GMapLinkInfoNotify), (p, s) => new GMapLinkInfoNotifyImpl(p, s));
            	ProtoManager.manager.Register(PID.GMapPingRespone, typeof(GMapPingRespone), (p, s) => new GMapPingResponeImpl(p, s));
            	ProtoManager.manager.Register(PID.GMapResourceLoadNotify, typeof(GMapResourceLoadNotify), (p, s) => new GMapResourceLoadNotifyImpl(p, s));
            	ProtoManager.manager.Register(PID.GMapGameStartNotify, typeof(GMapGameStartNotify), (p, s) => new GMapGameStartNotifyImpl(p, s));
            	ProtoManager.manager.Register(PID.FightMapLoadProgressNotify, typeof(FightMapLoadProgressNotify), (p, s) => new FightMapLoadProgressNotifyImpl(p, s));
            	ProtoManager.manager.Register(PID.FightMapLoadProgressReport, typeof(FightMapLoadProgressReport), (p, s) => new FightMapLoadProgressReportImpl(p, s));
            	ProtoManager.manager.Register(PID.GSrvGunFireReport, typeof(GSrvGunFireReport), (p, s) => new GSrvGunFireReportImpl(p, s));
            	ProtoManager.manager.Register(PID.GSrvPhysicAttackReport, typeof(GSrvPhysicAttackReport), (p, s) => new GSrvPhysicAttackReportImpl(p, s));
            	ProtoManager.manager.Register(PID.GSrvSkillCastReport, typeof(GSrvSkillCastReport), (p, s) => new GSrvSkillCastReportImpl(p, s));
            	ProtoManager.manager.Register(PID.GSrvTrapProcReport, typeof(GSrvTrapProcReport), (p, s) => new GSrvTrapProcReportImpl(p, s));
            	ProtoManager.manager.Register(PID.GMapQuizReplyReport, typeof(GMapQuizReplyReport), (p, s) => new GMapQuizReplyReportImpl(p, s));
            } 
            catch (System.Exception e) 
            {
                Console.Write(e.Message);
            }
        }
    }
}
