﻿// using Telegami.Middlewares;
//
// namespace Telegami.Sessions;
//
// public class TelegamiSessionMiddleware : ITelegamiMiddleware
// {
//     private readonly ITelegamiSessionsProvider _telegamiSessionsProvider;
//
//     public TelegamiSessionMiddleware(ITelegamiSessionsProvider telegamiSessionsProvider)
//     {
//         _telegamiSessionsProvider = telegamiSessionsProvider;
//     }
//
//     public async Task InvokeAsync(MessageContext ctx, MessageContextDelegate next)
//     {
//         var key = TelegamiSessionKey.From(ctx);
//         var session = await _telegamiSessionsProvider.GetAsync(key);
//
//         if (session is not null)
//         {
//             ctx.Session = session;
//         }
//         else
//         {
//             var newSession = new TelegamiSession();
//             ctx.Session = newSession;
//             await _telegamiSessionsProvider.SetAsync(key, newSession);
//         }
//
//         try
//         {
//             await next(ctx);
//         }
//         finally
//         {
//             await _telegamiSessionsProvider.SetAsync(key, ctx.Session);
//         }
//     }
// }