import {StrictMode, useEffect} from 'react'
import {createRoot} from 'react-dom/client'
import {BrowserRouter, Routes, Route, Navigate, useParams} from 'react-router';
import {useTranslation} from 'react-i18next';

import "./i18n";
import './index.css';

import Login from './pages/Login.tsx';
import Issuance from './pages/Issuance.tsx';
import Done from "./pages/Done.tsx";

// Wrapper that sets the language based on the URL
// from IBAN issuer: https://github.com/privacybydesign/yivi-iban-issuer/tree/main/react-cra
function LanguageRouter() {
    const {lang} = useParams();
    const {i18n} = useTranslation();

    useEffect(() => {
        if (lang && i18n.language !== lang) {
            i18n.changeLanguage(lang);
        }
    }, [lang, i18n]);

    return (
        <Routes>
            <Route path=":slug" element={<Login/>}/>
            <Route path=":slug/load-attributes" element={<Issuance/>}/>
            <Route path=":slug/success" element={<Done/>}/>
        </Routes>
    );
}

// Redirect /:slug to /:lang/:slug
function DefaultLanguageRedirect() {
    const {i18n} = useTranslation();
    const {slug} = useParams();

    return <Navigate to={`/${i18n.language}/${slug}`} replace/>;
}

createRoot(document.getElementById('root')!).render(
    <StrictMode>
        <BrowserRouter>
            <Routes>
                <Route path="/:slug" element={<DefaultLanguageRedirect/>}/>
                <Route path=":lang/*" element={<LanguageRouter/>}/>
            </Routes>
        </BrowserRouter>
    </StrictMode>,
)
